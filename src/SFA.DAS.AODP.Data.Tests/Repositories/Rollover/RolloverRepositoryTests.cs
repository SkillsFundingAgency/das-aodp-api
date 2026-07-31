using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class RolloverRepositoryTests
{
    private readonly IFixture _fixture;

    public RolloverRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    private static ApplicationDbContext CreateDb(string name)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"{name}_{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;

        return new ApplicationDbContext(options);
    }


    [Fact]
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsEmptyResult_When_NoRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Rollover_NoRecords_" + Guid.NewGuid())
            .Options;

        await using var db = new ApplicationDbContext(options);
        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetRolloverWorkflowCandidatesCountAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsPagedOrderedMappedData()
    {
        // Arrange 
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Rollover_Paged_" + Guid.NewGuid())
            .Options;

        var now = DateTime.UtcNow;
        var e1 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-2), null, now.AddDays(-2));
        var e2 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-1), null, now.AddDays(-1));
        var e3 = Data.Entities.Rollover.RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { e1, e2, e3 });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var db = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(db);

            // Act 
            var result = await sut.GetRolloverWorkflowCandidatesCountAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(3, result);

        }
    }

    [Fact]
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsEmpty_When_NoRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Rollover_NoRecords_" + Guid.NewGuid())
            .Options;

        await using var db = new ApplicationDbContext(options);
        var sut = new RolloverRepository(db);

        // Act
        var result = (await sut.GetAllRolloverWorkflowCandidatesAsync(TestContext.Current.CancellationToken)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllRolloverWorkflowCandidatesAsync_ReturnsAllRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Rollover_AllRecords_" + Guid.NewGuid())
            .Options;

        var now = DateTime.UtcNow;
        var e1 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-3), null, now.AddDays(-3));
        var e2 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-2), null, now.AddDays(-2));
        var e3 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now.AddDays(-1), null, now.AddDays(-1));

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { e1, e2, e3 });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var db = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(db);

            // Act
            var result = (await sut.GetAllRolloverWorkflowCandidatesAsync(TestContext.Current.CancellationToken)).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.CreatedAt == e1.CreatedAt);
            Assert.Contains(result, r => r.CreatedAt == e2.CreatedAt);
            Assert.Contains(result, r => r.CreatedAt == e3.CreatedAt);
        }
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesP1ChecksAsync_ReturnsEmpty_When_NoRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("RolloverP1_NoRecords_" + Guid.NewGuid())
            .Options;

        await using var db = new ApplicationDbContext(options);
        var sut = new RolloverRepository(db);

        // Act
        var result = (await sut.GetRolloverWorkflowCandidatesP1ChecksAsync(TestContext.Current.CancellationToken)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesP1ChecksAsync_ReturnsAllRecords()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("RolloverP1_AllRecords_" + Guid.NewGuid())
            .Options;

        var now = DateTime.UtcNow;
        var e1 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = Guid.NewGuid(),
            QualificationVersionId = Guid.NewGuid(),
            FundingOfferId = Guid.NewGuid(),
            AcademicYear = "2025/26",
            IncludedInP1Export = true,
            IncludedInFinalUpload = false,
            CurrentFundingEndDate = now.Date,
            ProposedFundingEndDate = now.Date.AddYears(1),
            FundingStream = "FS1",
            RolloverRound = 1,
            FundingEndDateThreshold = now.Date.AddDays(-10),
            LatestFundingApprovalEndDate = now.Date.AddDays(-20),
            OperationalStartDate = now.Date.AddYears(-1),
            OperationalEndDate = now.Date.AddMonths(6),
            OperationalEndDateThreshold = now.Date.AddDays(-5),
            OfferedInEngland = true,
            IsOnDefundingList = false
        };

        var e2 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = Guid.NewGuid(),
            QualificationVersionId = Guid.NewGuid(),
            FundingOfferId = Guid.NewGuid(),
            AcademicYear = "2026/27",
            IncludedInP1Export = false,
            IncludedInFinalUpload = true,
            CurrentFundingEndDate = now.Date,
            ProposedFundingEndDate = null,
            FundingStream = null,
            RolloverRound = 2,
            FundingEndDateThreshold = now.Date.AddDays(30),
            LatestFundingApprovalEndDate = now.Date.AddDays(60),
            OperationalStartDate = now.Date,
            OperationalEndDate = null,
            OperationalEndDateThreshold = now.Date.AddDays(15),
            OfferedInEngland = false,
            IsOnDefundingList = true
        };

        await using (var db = new ApplicationDbContext(options))
        {
            await db.RolloverWorkflowCandidatesP1Checks.AddRangeAsync(new[] { e1, e2 });
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using (var db = new ApplicationDbContext(options))
        {
            var sut = new RolloverRepository(db);

            // Act
            var result = (await sut.GetRolloverWorkflowCandidatesP1ChecksAsync(TestContext.Current.CancellationToken)).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.WorkflowCandidateId == e1.WorkflowCandidateId);
            Assert.Contains(result, r => r.WorkflowCandidateId == e2.WorkflowCandidateId);

            var fetched1 = result.Single(r => r.WorkflowCandidateId == e1.WorkflowCandidateId);
            Assert.Equal(e1.AcademicYear, fetched1.AcademicYear);
            Assert.Equal(e1.FundingStream, fetched1.FundingStream);
            Assert.Equal(e1.IsOnDefundingList, fetched1.IsOnDefundingList);

            var fetched2 = result.Single(r => r.WorkflowCandidateId == e2.WorkflowCandidateId);
            Assert.Equal(e2.AcademicYear, fetched2.AcademicYear);
            Assert.Equal(e2.FundingStream, fetched2.FundingStream);
            Assert.Equal(e2.IsOnDefundingList, fetched2.IsOnDefundingList);
        }
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidatesByRunId_ReturnsEmpty_When_NoRecords()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetRolloverWorkflowCandidatesByRunId_ReturnsEmpty_When_NoRecords));
        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetRolloverWorkflowCandidatesByRunId(Guid.NewGuid(), TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetRolloverWorkflowCandidatesByRunId_FiltersByRunId_And_IncludedInP1Export()
    {
        var runId = Guid.NewGuid();
        var otherRunId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        await using var db = CreateDb(nameof(GetRolloverWorkflowCandidatesByRunId_FiltersByRunId_And_IncludedInP1Export));

        // Minimal qualification + version + funding offer so projection works
        var qualification = new Qualification { Id = Guid.NewGuid(), Qan = "QAN-123" };
        var organisation = new AwardingOrganisation { Id = Guid.NewGuid(), NameOfqual = "Org" };
        var qualVersion = new QualificationVersions
        {
            Id = Guid.NewGuid(),
            QualificationId = qualification.Id,
            Qualification = qualification,
            Organisation = organisation,
            EqfLevel = "Efq",
            Level ="Level",
            Ssa ="ssa",
            Status ="Status",
            SubLevel = "Sublevel",
            Type = "Type"
        };
        var fundingOffer = new FundingOffer { Id = Guid.NewGuid(), Name = "Funding A", DisplayName = "Funding A" };

        var candidate = RolloverCandidates.CreateInitialRound(
            qualVersion.Id, fundingOffer.Id, "2024/25", now);
        candidate.QualificationVersion = qualVersion;
        candidate.FundingOffer = fundingOffer;

        var included = RolloverWorkflowCandidate.Create(
            runId, candidate.Id, qualVersion.Id, fundingOffer.Id,
            "2024/25", 1, now, null, now);
        included.RolloverCandidates = candidate;

        var excluded = RolloverWorkflowCandidate.Create(
            runId, candidate.Id, qualVersion.Id, fundingOffer.Id,
            "2024/25", 1, now, null, now);
        typeof(RolloverWorkflowCandidate)
            .GetProperty("IncludedInP1Export")!
            .SetValue(excluded, false);
        excluded.RolloverCandidates = candidate;

        var differentRun = RolloverWorkflowCandidate.Create(
            otherRunId, candidate.Id, qualVersion.Id, fundingOffer.Id,
            "2024/25", 1, now, null, now);
        differentRun.RolloverCandidates = candidate;

        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(qualVersion, TestContext.Current.CancellationToken);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(fundingOffer, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddAsync(candidate, TestContext.Current.CancellationToken);
        await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { included, excluded, differentRun }, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var result = (await sut.GetRolloverWorkflowCandidatesByRunId(runId, TestContext.Current.CancellationToken)).ToList();
        var row = Assert.Single(result);
        Assert.Equal("QAN-123", row.QAN);
        Assert.Equal("Funding A", row.FundingStreamName);
    }


    [Fact]
    public async Task GetRolloverWorkflowCandidatesByRunId_OrdersByQAN()
    {
        await using var db = CreateDb(nameof(GetRolloverWorkflowCandidatesByRunId_OrdersByQAN));

        var runId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var qualificationA = new Qualification { Id = Guid.NewGuid(), Qan = "A111" };
        var qualificationB = new Qualification { Id = Guid.NewGuid(), Qan = "B222" };
        var organisation = new AwardingOrganisation { Id = Guid.NewGuid(), NameOfqual = "Org" };

        var versionA = new QualificationVersions { Id = Guid.NewGuid(), Qualification = qualificationA, Organisation = organisation,EqfLevel = "Efq", Level = "Level", Ssa = "ssa", Status = "Status",SubLevel = "Sublevel",Type = "Type"};
        var versionB = new QualificationVersions { Id = Guid.NewGuid(), Qualification = qualificationB, Organisation = organisation, EqfLevel = "Efq", Level = "Level", Ssa = "ssa", Status = "Status", SubLevel = "Sublevel", Type = "Type" };

        var funding = new FundingOffer { Id = Guid.NewGuid(), Name = "Funding", DisplayName = "Funding" };

        var candidateA = RolloverCandidates.CreateInitialRound(versionA.Id, funding.Id, "2024/25", now);
        candidateA.QualificationVersion = versionA;
        candidateA.FundingOffer = funding;

        var candidateB = RolloverCandidates.CreateInitialRound(versionB.Id, funding.Id, "2024/25", now);
        candidateB.QualificationVersion = versionB;
        candidateB.FundingOffer = funding;

        var wcB = RolloverWorkflowCandidate.Create(runId, candidateB.Id, versionB.Id, funding.Id, "2024/25", 1, now, null, now);
        wcB.RolloverCandidates = candidateB;

        var wcA = RolloverWorkflowCandidate.Create(runId, candidateA.Id, versionA.Id, funding.Id, "2024/25", 1, now, null, now);
        wcA.RolloverCandidates = candidateA;

        await db.Qualification.AddRangeAsync(new[] { qualificationA, qualificationB });
        await db.QualificationVersions.AddRangeAsync(new[] { versionA, versionB });
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(funding, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddRangeAsync(new[] { candidateA, candidateB });
        await db.RolloverWorkflowCandidates.AddRangeAsync(new[] { wcB, wcA }, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var result = (await sut.GetRolloverWorkflowCandidatesByRunId(runId, TestContext.Current.CancellationToken)).ToList();
        Assert.Equal("A111", result[0].QAN);
        Assert.Equal("B222", result[1].QAN);
    }


    [Fact]
    public async Task UpdateRolloverWorkflowCandidatesAsync_DoesNothing_When_EmptyList()
    {
        // Arrange
        await using var db = CreateDb(nameof(UpdateRolloverWorkflowCandidatesAsync_DoesNothing_When_EmptyList));
        var sut = new RolloverRepository(db);

        // Act
        await sut.UpdateRolloverWorkflowCandidatesAsync(new List<RolloverWorkflowCandidate>(), TestContext.Current.CancellationToken);

        // Assert - should not throw and nothing saved
        var count = await db.RolloverWorkflowCandidates.CountAsync(TestContext.Current.CancellationToken);
        count.ShouldBe(0);
    }

    [Fact]
    public async Task UpdateRolloverWorkflowCandidatesAsync_UpdatesAndSaves_When_ListProvided()
    {
        // Arrange
        await using var db = CreateDb(nameof(UpdateRolloverWorkflowCandidatesAsync_UpdatesAndSaves_When_ListProvided));

        var now = DateTime.UtcNow;
        var entity = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);
        await db.RolloverWorkflowCandidates.AddAsync(entity, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // modify the entity via its public API to change state
        var checks = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = entity.Id,
            QualificationVersionId = entity.QualificationVersionId,
            FundingOfferId = entity.FundingOfferId,
            AcademicYear = entity.AcademicYear,
            IncludedInP1Export = true,
            IncludedInFinalUpload = false,
            CurrentFundingEndDate = now,
            ProposedFundingEndDate = now.AddYears(1),
            FundingStream = "FS",
            RolloverRound = entity.RolloverRound,
            FundingEndDateThreshold = now.AddDays(-1),
            LatestFundingApprovalEndDate = now.AddDays(10),
            OperationalStartDate = now.AddYears(-1),
            OperationalEndDate = now.AddYears(1),
            OperationalEndDateThreshold = now.AddDays(-5),
            OfferedInEngland = true,
            IntentionToSeekFundingInEngland = true,
            IsOnDefundingList = false
        };

        // Process checks to change PassP1 to true
        entity.ProcessP1Checks(checks);

        var sut = new RolloverRepository(db);

        // Act
        await sut.UpdateRolloverWorkflowCandidatesAsync(new[] { entity }, TestContext.Current.CancellationToken);

        // Assert
        var fetched = await db.RolloverWorkflowCandidates.FirstAsync(TestContext.Current.CancellationToken);
        fetched.PassP1.ShouldBeTrue();
    }

    [Fact]
    public async Task GetRolloverCandidatesAsync_ReturnsOnlyActive_With_ProperProjection()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetRolloverCandidatesAsync_ReturnsOnlyActive_With_ProperProjection));

        var qualification = new Qualification { Id = Guid.NewGuid(), Qan = "Q-1" };
        var qualVersion = new QualificationVersions { Id = Guid.NewGuid(), QualificationId = qualification.Id, Qualification = qualification, EqfLevel = "Efq", Level = "Level", Ssa = "ssa", Status = "Status", SubLevel = "Sublevel", Type = "Type" };
        var funding = new FundingOffer { Id = Guid.NewGuid(), Name = "FundX", DisplayName = "Fund X"};

        var active = RolloverCandidates.CreateInitialRound(qualVersion.Id, funding.Id, "2024/25", DateTime.UtcNow);
        // CreateInitialRound sets IsActive = true so no setter needed

        active.QualificationVersion = qualVersion;
        active.FundingOffer = funding;

        var inactive = RolloverCandidates.CreateInitialRound(qualVersion.Id, funding.Id, "2024/25", DateTime.UtcNow);
        // create an inactive record by adding then setting IsActive via EF Core change tracker (avoids private setter reflection)
        inactive.QualificationVersion = qualVersion;
        inactive.FundingOffer = funding;

        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(qualVersion, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(funding, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddRangeAsync(new[] { active, inactive }, TestContext.Current.CancellationToken);

        // set inactive flag using EF property API
        db.Entry(inactive).Property("IsActive").CurrentValue = false;

        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = (await sut.GetRolloverCandidatesAsync(TestContext.Current.CancellationToken)).ToList();

        // Assert
        result.Count.ShouldBe(1);
        var dto = result.Single();
        dto.Id.ShouldBe(active.Id);
        dto.QualificationVersionId.ShouldBe(active.QualificationVersionId);
        dto.FundingOfferId.ShouldBe(active.FundingOfferId);
        dto.FundingOfferName.ShouldBe("Fund X");
        dto.QualificationNumber.ShouldBe("Q-1");
        dto.AcademicYear.ShouldBe(active.AcademicYear);
    }


    [Fact]
    public async Task GetRolloverCandidatesByIdsAsync_ReturnsOnlyActiveAndProjectedFields()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetRolloverCandidatesByIdsAsync_ReturnsOnlyActiveAndProjectedFields));

        var now = DateTime.UtcNow;
        var active = RolloverCandidates.CreateInitialRound(Guid.NewGuid(), Guid.NewGuid(), "2024/25", now);
        var inactive = RolloverCandidates.CreateInitialRound(Guid.NewGuid(), Guid.NewGuid(), "2024/25", now);

        // set some date fields and rollover round via EF property API
        var prevDate = now.AddMonths(-6);
        var newDate = now.AddYears(1);

        db.RolloverCandidates.Add(active);
        db.RolloverCandidates.Add(inactive);

        // mark inactive record IsActive = false
        db.Entry(inactive).Property("IsActive").CurrentValue = false;

        // set previous and new funding end dates on active
        db.Entry(active).Property("PreviousFundingEndDate").CurrentValue = prevDate;
        db.Entry(active).Property("NewFundingEndDate").CurrentValue = newDate;
        db.Entry(active).Property("RolloverRound").CurrentValue = 2;

        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = (await sut.GetRolloverCandidatesByIdsAsync(new[] { active.Id, inactive.Id }, TestContext.Current.CancellationToken)).ToList();

        // Assert
        result.Count.ShouldBe(1);
        var dto = result.Single();
        dto.Id.ShouldBe(active.Id);
        dto.QualificationVersionId.ShouldBe(active.QualificationVersionId);
        dto.FundingOfferId.ShouldBe(active.FundingOfferId);
        dto.RolloverRound.ShouldBe(2);
        dto.AcademicYear.ShouldBe(active.AcademicYear);
        dto.PreviousFundingEndDate.ShouldBe(prevDate);
        dto.NewFundingEndDate.ShouldBe(newDate);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunAsync_AddsEntityAndReturnsId()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunAsync_AddsEntityAndReturnsId));
        var sut = new RolloverRepository(db);

        var workflowRun = RolloverWorkflowRun.Create(
            "2024/25",
            SFA.DAS.AODP.Data.Entities.Rollover.Enums.SelectionMethod.QueryBuilder,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow.AddYears(1),
            "test.user",
            DateTime.UtcNow);

        // Act
        var id = await sut.CreateRolloverWorkflowRunAsync(workflowRun, TestContext.Current.CancellationToken);

        // Assert
        id.ShouldBe(workflowRun.Id);
        var persisted = await db.RolloverWorkflowRuns.FirstOrDefaultAsync(r => r.Id == id, TestContext.Current.CancellationToken);
        persisted.ShouldNotBeNull();
        persisted!.AcademicYear.ShouldBe("2024/25");
    }

    [Fact]
    public async Task CreateRolloverWorkflowCandidatesAsync_ReplacesExistingAndAddsIncoming()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowCandidatesAsync_ReplacesExistingAndAddsIncoming));

        var candidateRecordId = Guid.NewGuid();

        var existing = RolloverWorkflowCandidate.Create(Guid.NewGuid(), candidateRecordId, Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, DateTime.UtcNow, null, DateTime.UtcNow);
        await db.RolloverWorkflowCandidates.AddAsync(existing, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var incoming1 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), candidateRecordId, existing.QualificationVersionId, existing.FundingOfferId, "2024/25", 1, DateTime.UtcNow, null, DateTime.UtcNow);
        var incoming2 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, DateTime.UtcNow, null, DateTime.UtcNow);

        var sut = new RolloverRepository(db);

        // Act
        await sut.CreateRolloverWorkflowCandidatesAsync(new[] { incoming1, incoming2 }, TestContext.Current.CancellationToken);

        // Assert - existing should be removed and incoming present
        var all = await db.RolloverWorkflowCandidates.ToListAsync(TestContext.Current.CancellationToken);
        all.Count.ShouldBe(2);
        all.ShouldContain(x => x.RolloverWorkflowRunId == incoming1.RolloverWorkflowRunId && x.RolloverCandidatesId == incoming1.RolloverCandidatesId);
        all.ShouldContain(x => x.RolloverWorkflowRunId == incoming2.RolloverWorkflowRunId && x.RolloverCandidatesId == incoming2.RolloverCandidatesId);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRunFundingOffersAsync_AddsOffersAndSaves()
    {
        // Arrange
        await using var db = CreateDb(nameof(CreateRolloverWorkflowRunFundingOffersAsync_AddsOffersAndSaves));

        var runId = Guid.NewGuid();
        var offer1 = RolloverWorkflowRunFundingOffer.Create(runId, Guid.NewGuid());
        var offer2 = RolloverWorkflowRunFundingOffer.Create(runId, Guid.NewGuid());

        var sut = new RolloverRepository(db);

        // Act
        await sut.CreateRolloverWorkflowRunFundingOffersAsync(new[] { offer1, offer2 }, TestContext.Current.CancellationToken);

        // Assert
        var saved = await db.RolloverWorkflowRunFundingOffers.ToListAsync(TestContext.Current.CancellationToken);
        saved.Count.ShouldBe(2);
        saved.ShouldContain(x => x.RolloverWorkflowRunId == runId && x.FundingOfferId == offer1.FundingOfferId);
        saved.ShouldContain(x => x.RolloverWorkflowRunId == runId && x.FundingOfferId == offer2.FundingOfferId);
    }

    [Fact]
    public async Task SaveChangesAsync_ProxiesToContextAndPersists()
    {
        // Arrange
        await using var db = CreateDb(nameof(SaveChangesAsync_ProxiesToContextAndPersists));
        var sut = new RolloverRepository(db);

        var run = RolloverWorkflowRun.Create(
            "2024/25",
            SFA.DAS.AODP.Data.Entities.Rollover.Enums.SelectionMethod.FileUpload,
            null,
            null,
            null,
            "user",
            DateTime.UtcNow);

        db.RolloverWorkflowRuns.Add(run);

        // Act
        await sut.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var persisted = await db.RolloverWorkflowRuns.FirstOrDefaultAsync(r => r.Id == run.Id, TestContext.Current.CancellationToken);
        persisted.ShouldNotBeNull();
    }


    [Fact]
    public async Task GeRolloverWorkflowRunByIdAsync_ReturnsNull_When_NotFound()
    {
        // Arrange
        await using var db = CreateDb(nameof(GeRolloverWorkflowRunByIdAsync_ReturnsNull_When_NotFound));
        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GeRolloverWorkflowRunByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GeRolloverWorkflowRunByIdAsync_ReturnsEntity_When_Found()
    {
        // Arrange
        await using var db = CreateDb(nameof(GeRolloverWorkflowRunByIdAsync_ReturnsEntity_When_Found));
        var run = RolloverWorkflowRun.Create(
            "2024/25",
            SFA.DAS.AODP.Data.Entities.Rollover.Enums.SelectionMethod.FileUpload,
            null,
            null,
            null,
            "user",
            DateTime.UtcNow);
        db.RolloverWorkflowRuns.Add(run);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GeRolloverWorkflowRunByIdAsync(run.Id, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result!.Id.ShouldBe(run.Id);
        result.AcademicYear.ShouldBe("2024/25");
    }

    [Fact]
    public async Task GetFundingExtensionValidationContextAsync_Throws_When_NoRuns()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetFundingExtensionValidationContextAsync_Throws_When_NoRuns));
        var sut = new RolloverRepository(db);

        var incoming = new HashSet<CandidateKey> { new CandidateKey("Q-1", "FundX") };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await sut.GetFundingExtensionValidationContextAsync(incoming, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetRolloverCandidatesStatusAsync_ReturnsProjectedItems()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetRolloverCandidatesStatusAsync_ReturnsProjectedItems));

        var qualification = new Qualification { Id = Guid.NewGuid(), Qan = "Q-STAT" };
        var version = new QualificationVersions { Id = Guid.NewGuid(), QualificationId = qualification.Id, Qualification = qualification, Status = "Approved", Type = "Standard", Ssa = "001", Level = "3", SubLevel = "A", EqfLevel = "EQF3" };
        var funding = new FundingOffer { Id = Guid.NewGuid(), Name = "FundStat", DisplayName = "Fund Stat" };

        var candidate = RolloverCandidates.CreateInitialRound(version.Id, funding.Id, "2024/25", DateTime.UtcNow);
        candidate.QualificationVersion = version;
        candidate.FundingOffer = funding;

        // set status via EF shadow property if necessary
        db.Qualification.Add(qualification);
        db.QualificationVersions.Add(version);
        db.FundingOffers.Add(funding);
        db.RolloverCandidates.Add(candidate);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetRolloverCandidatesStatusAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        var item = result.SingleOrDefault(x => x.Qan == "Q-STAT" && x.FundingStreamName == "FundStat");
        item.ShouldNotBeNull();
        item!.RolloverStatus.ShouldBe(candidate.RolloverStatus);
    }

    [Fact]
    public async Task LoadRolloverCandidateGraphAsync_LoadsNavigationProperties()
    {
        // Arrange
        await using var db = CreateDb(nameof(LoadRolloverCandidateGraphAsync_LoadsNavigationProperties));

        var qualification = new Qualification { Id = Guid.NewGuid(), Qan = "Q-GRAPH" };
        var organisation = new AwardingOrganisation { Id = Guid.NewGuid(), NameOfqual = "OrgG" };
        var version = new QualificationVersions
        {
            Id = Guid.NewGuid(),
            QualificationId = qualification.Id,
            Qualification = qualification,
            Organisation = organisation,
            AwardingOrganisationId = organisation.Id,
            // Fill required non-nullable properties so EF can save the entity
            Status = "Active",
            Type = "TypeA",
            Ssa = "SSA",
            Level = "1",
            SubLevel = "A",
            EqfLevel = "E1"
        };
        var funding = new FundingOffer { Id = Guid.NewGuid(), Name = "FundG", DisplayName = "Funding G" };

        var candidate = RolloverCandidates.CreateInitialRound(version.Id, funding.Id, "2024/25", DateTime.UtcNow);
        candidate.QualificationVersion = version;
        candidate.FundingOffer = funding;

        await db.Qualification.AddAsync(qualification, TestContext.Current.CancellationToken);
        await db.QualificationVersions.AddAsync(version, TestContext.Current.CancellationToken);
        await db.AwardingOrganisation.AddAsync(organisation, TestContext.Current.CancellationToken);
        await db.FundingOffers.AddAsync(funding, TestContext.Current.CancellationToken);
        await db.RolloverCandidates.AddAsync(candidate, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        var keys = new List<CandidateKey> { new CandidateKey("Q-GRAPH", "FundG") };

        // Act
        var list = await sut.LoadRolloverCandidateGraphAsync(keys, TestContext.Current.CancellationToken);

        // Assert
        list.ShouldNotBeNull();
        var fetched = list.Single();
        fetched.QualificationVersion.ShouldNotBeNull();
        fetched.FundingOffer.ShouldNotBeNull();
        fetched.QualificationVersion.Qualification.Qan.ShouldBe("Q-GRAPH");
        fetched.FundingOffer.Name.ShouldBe("FundG");
    }

    [Fact]
    public async Task DeleteAllWorkflowCandidatesAsync_RemovesAllRecords()
    {
        // Arrange
        await using var db = CreateDb(nameof(DeleteAllWorkflowCandidatesAsync_RemovesAllRecords));

        var now = DateTime.UtcNow;
        var e1 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);
        var e2 = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", 1, now, null, now);

        await db.RolloverWorkflowCandidates.AddRangeAsync(e1, e2);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        await sut.DeleteAllWorkflowCandidatesAsync(TestContext.Current.CancellationToken);

        // Assert
        var count = await db.RolloverWorkflowCandidates.CountAsync(TestContext.Current.CancellationToken);
        count.ShouldBe(0);
    }

    [Fact]
    public async Task GetLatestWorkflowRunIdAsync_ReturnsEmpty_When_NoRuns()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetLatestWorkflowRunIdAsync_ReturnsEmpty_When_NoRuns));
        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetLatestWorkflowRunIdAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(Guid.Empty);
    }

    [Fact]
    public async Task GetLatestWorkflowRunIdAsync_ReturnsLatestId()
    {
        // Arrange
        await using var db = CreateDb(nameof(GetLatestWorkflowRunIdAsync_ReturnsLatestId));
        var older = RolloverWorkflowRun.Create("2023/24", SFA.DAS.AODP.Data.Entities.Rollover.Enums.SelectionMethod.FileUpload, DateTime.UtcNow.AddDays(-2), null, null, "u", DateTime.UtcNow.AddDays(-2));
        var newer = RolloverWorkflowRun.Create("2024/25", SFA.DAS.AODP.Data.Entities.Rollover.Enums.SelectionMethod.FileUpload, DateTime.UtcNow, null, null, "u2", DateTime.UtcNow);

        db.RolloverWorkflowRuns.Add(older);
        db.RolloverWorkflowRuns.Add(newer);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var sut = new RolloverRepository(db);

        // Act
        var result = await sut.GetLatestWorkflowRunIdAsync(TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(newer.Id);
    }

}