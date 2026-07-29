using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using SFA.DAS.AODP.Testing.Stubs;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class RolloverCandidateReconcilerTests
{
    private static readonly DateTime Now = new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ReconcileAsync_CreatesOneCandidatePerFundingOffer()
    {
        await using var context = CreateContext();
        var qualificationId = Guid.NewGuid();
        var fundingOfferIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var keys = fundingOfferIds
            .Select(x => new FundingChangeKey(
                RolloverSourceTypes.Ofqual,
                qualificationId,
                x,
                "2026/27"))
            .ToList();
        var eligibilityRepository = CreateEligibilityRepository(keys, true);
        var sut = CreateSut(context, eligibilityRepository);

        var result = await sut.ReconcileAsync(keys, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, result.Created);
        Assert.Equal(3, await context.RolloverCandidates.CountAsync(
            TestContext.Current.CancellationToken));
        Assert.All(
            await context.RolloverCandidates.ToListAsync(TestContext.Current.CancellationToken),
            candidate =>
            {
                Assert.Equal(qualificationId, candidate.SourceQualificationId);
                Assert.Contains(candidate.FundingOfferId, fundingOfferIds);
                Assert.True(candidate.IsActive);
            });
    }

    [Fact]
    public async Task ReconcileAsync_DeactivatesOnlyTheChangedFundingOffer()
    {
        await using var context = CreateContext();
        var qualificationId = Guid.NewGuid();
        var fundingOfferIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var candidates = fundingOfferIds
            .Select(x => RolloverCandidates.CreateInitialRound(
                RolloverSourceTypes.Ofqual,
                qualificationId,
                x,
                "2026/27",
                Now))
            .ToList();
        context.RolloverCandidates.AddRange(candidates);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var key = new FundingChangeKey(
            RolloverSourceTypes.Ofqual,
            qualificationId,
            fundingOfferIds[1],
            "2026/27");
        var eligibilityRepository = CreateEligibilityRepository([key], false);
        var sut = CreateSut(context, eligibilityRepository);

        var result = await sut.ReconcileAsync([key], TestContext.Current.CancellationToken);

        Assert.Equal(1, result.Deactivated);
        Assert.True(candidates[0].IsActive);
        Assert.False(candidates[1].IsActive);
        Assert.True(candidates[2].IsActive);
    }

    [Fact]
    public async Task ReconcileAsync_QualificationLevelIneligibilityDeactivatesAllFundingOffers()
    {
        await using var context = CreateContext();
        var qualificationId = Guid.NewGuid();
        var keys = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
            .Select(fundingOfferId => new FundingChangeKey(
                RolloverSourceTypes.Ofqual,
                qualificationId,
                fundingOfferId,
                "2026/27"))
            .ToList();
        context.RolloverCandidates.AddRange(keys.Select(key =>
            RolloverCandidates.CreateInitialRound(
                key.SourceType,
                key.SourceQualificationId,
                key.FundingOfferId,
                key.AcademicYear!,
                Now)));
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var sut = CreateSut(
            context,
            CreateEligibilityRepository(keys, false));

        var result = await sut.ReconcileAsync(
            keys,
            TestContext.Current.CancellationToken);

        Assert.Equal(3, result.Deactivated);
        Assert.All(
            await context.RolloverCandidates.ToListAsync(
                TestContext.Current.CancellationToken),
            candidate => Assert.False(candidate.IsActive));
    }

    [Fact]
    public async Task ReconcileAsync_ReactivatesCandidateWithoutRevalidatingHistoricalWorkflow()
    {
        await using var context = CreateContext();
        var key = new FundingChangeKey(
            RolloverSourceTypes.Qaa,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2026/27");
        var candidate = RolloverCandidates.CreateInitialRound(
            key.SourceType,
            key.SourceQualificationId,
            key.FundingOfferId,
            key.AcademicYear!,
            Now);
        var workflowCandidate = RolloverWorkflowCandidate.Create(
            Guid.NewGuid(),
            candidate.Id,
            key.SourceType,
            key.SourceQualificationId,
            key.FundingOfferId,
            key.AcademicYear!,
            1,
            Now,
            null,
            Now);
        context.RolloverCandidates.Add(candidate);
        context.RolloverWorkflowCandidates.Add(workflowCandidate);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var eligibilityRepository = new Mock<IRolloverFundingEligibilityRepository>();
        eligibilityRepository
            .SetupSequence(x => x.GetAsync(
                It.IsAny<IReadOnlyCollection<FundingChangeKey>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateEligibility(key, false)])
            .ReturnsAsync([CreateEligibility(key, true)]);
        var sut = CreateSut(context, eligibilityRepository);

        await sut.ReconcileAsync([key], TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        await sut.ReconcileAsync([key], TestContext.Current.CancellationToken);

        Assert.True(candidate.IsActive);
        Assert.Equal(RolloverStatus.NeedsReview, candidate.RolloverStatus);
        Assert.NotNull(workflowCandidate.InvalidatedAt);
        Assert.NotNull(workflowCandidate.InvalidationReason);
    }

    [Fact]
    public async Task ReconcileAsync_IsolatesIdenticalIdsBySourceType()
    {
        await using var context = CreateContext();
        var qualificationId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var ofqualCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual,
            qualificationId,
            fundingOfferId,
            "2026/27",
            Now);
        var qaaCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Qaa,
            qualificationId,
            fundingOfferId,
            "2026/27",
            Now);
        context.RolloverCandidates.AddRange(ofqualCandidate, qaaCandidate);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var qaaKey = new FundingChangeKey(
            RolloverSourceTypes.Qaa,
            qualificationId,
            fundingOfferId,
            "2026/27");
        var sut = CreateSut(context, CreateEligibilityRepository([qaaKey], false));

        await sut.ReconcileAsync([qaaKey], TestContext.Current.CancellationToken);

        Assert.True(ofqualCandidate.IsActive);
        Assert.False(qaaCandidate.IsActive);
    }

    [Fact]
    public async Task CandidateIdentity_EnforcesSourceQualificationOfferYearAndRoundUniqueness()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        var fundingOffer = new FundingOffer
        {
            Id = Guid.NewGuid(),
            Name = "Funding A",
            DisplayName = "Funding A"
        };
        var qualificationId = Guid.NewGuid();
        context.FundingOffers.Add(fundingOffer);
        context.RolloverCandidates.AddRange(
            RolloverCandidates.CreateInitialRound(
                RolloverSourceTypes.Ofqual,
                qualificationId,
                fundingOffer.Id,
                "2026/27",
                Now),
            RolloverCandidates.CreateInitialRound(
                RolloverSourceTypes.Ofqual,
                qualificationId,
                fundingOffer.Id,
                "2026/27",
                Now));

        await Assert.ThrowsAsync<DbUpdateException>(() =>
            context.SaveChangesAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CandidateIdentity_AllowsIdenticalIdsForDifferentSourceTypes()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
        var fundingOffer = new FundingOffer
        {
            Id = Guid.NewGuid(),
            Name = "Funding A",
            DisplayName = "Funding A"
        };
        var qualificationId = Guid.NewGuid();
        context.FundingOffers.Add(fundingOffer);
        context.RolloverCandidates.AddRange(
            RolloverCandidates.CreateInitialRound(
                RolloverSourceTypes.Ofqual,
                qualificationId,
                fundingOffer.Id,
                "2026/27",
                Now),
            RolloverCandidates.CreateInitialRound(
                RolloverSourceTypes.Qaa,
                qualificationId,
                fundingOffer.Id,
                "2026/27",
                Now));

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.Equal(2, await context.RolloverCandidates.CountAsync(
            TestContext.Current.CancellationToken));
    }

    private static RolloverCandidateReconciler CreateSut(
        ApplicationDbContext context,
        Mock<IRolloverFundingEligibilityRepository> eligibilityRepository)
    {
        ISystemClockProvider clock = new FakeSystemClockProvider(DateOnly.FromDateTime(Now));
        return new RolloverCandidateReconciler(context, eligibilityRepository.Object, clock);
    }

    private static Mock<IRolloverFundingEligibilityRepository> CreateEligibilityRepository(
        IReadOnlyCollection<FundingChangeKey> keys,
        bool isEligible)
    {
        var mock = new Mock<IRolloverFundingEligibilityRepository>();
        mock.Setup(x => x.GetAsync(
                It.IsAny<IReadOnlyCollection<FundingChangeKey>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(keys.Select(x => CreateEligibility(x, isEligible)).ToList());
        return mock;
    }

    private static RolloverFundingEligibility CreateEligibility(
        FundingChangeKey key,
        bool isEligible)
    {
        return new RolloverFundingEligibility(
            key,
            key.AcademicYear!,
            new DateOnly(2027, 7, 31),
            isEligible);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"RolloverCandidateReconciler_{Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }
}
