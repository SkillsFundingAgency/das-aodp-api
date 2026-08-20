using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Funding;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Providers;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Repositories.Rollover;

public class FundingDomainEventDispatcherTests : UnitTest
{
    private static readonly DateTime Now = new(2026, 7, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task DispatchAsync_WhenOfqualFundingMoves_PreservesCandidateAndInvalidatesWorkflow()
    {
        // Arrange
        await using var context = CreateContext();
        var oldVersionId = Guid.NewGuid();
        var newVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var candidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual,
            oldVersionId,
            fundingOfferId,
            "2025/26",
            Now.AddDays(-2));
        candidate.RefreshFunding(new DateOnly(2026, 7, 31), Now.AddDays(-2));
        candidate.SetExtended(new DateTime(2027, 7, 31));
        var workflow = RolloverWorkflowCandidate.Create(
            Guid.NewGuid(),
            candidate.Id,
            RolloverSourceTypes.Ofqual,
            oldVersionId,
            fundingOfferId,
            candidate.AcademicYear,
            candidate.RolloverRound,
            new DateTime(2026, 7, 31),
            new DateTime(2027, 7, 31),
            Now.AddDays(-1));
        context.RolloverCandidates.Add(candidate);
        context.RolloverWorkflowCandidates.Add(workflow);
        await context.SaveChangesAsync(CancellationToken);
        var sut = CreateDispatcher();

        // Act
        await sut.DispatchAsync(
            context,
            [new FundingChangedDomainEvent(
                RolloverSourceTypes.Ofqual,
                newVersionId,
                fundingOfferId,
                oldVersionId)],
            CancellationToken);

        // Assert
        candidate.SourceQualificationId.ShouldBe(newVersionId);
        candidate.RolloverStatus.ShouldBe(RolloverStatus.Extended);
        candidate.NewFundingEndDate.ShouldBe(new DateTime(2027, 7, 31));
        candidate.PreviousFundingEndDate.ShouldBe(new DateTime(2026, 7, 31));
        candidate.IsActive.ShouldBeTrue();
        workflow.InvalidatedAt.ShouldBe(Now);
        workflow.InvalidationReason.ShouldNotBeNull().ShouldContain("newer qualification version");
    }

    [Fact]
    public async Task DispatchAsync_WhenTargetCandidateExists_DeactivatesOldCandidate()
    {
        // Arrange
        await using var context = CreateContext();
        var oldVersionId = Guid.NewGuid();
        var newVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var oldCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual,
            oldVersionId,
            fundingOfferId,
            "2025/26",
            Now.AddDays(-2));
        var targetCandidate = RolloverCandidates.CreateInitialRound(
            RolloverSourceTypes.Ofqual,
            newVersionId,
            fundingOfferId,
            "2025/26",
            Now.AddDays(-1));
        context.RolloverCandidates.AddRange(oldCandidate, targetCandidate);
        await context.SaveChangesAsync(CancellationToken);
        var sut = CreateDispatcher();

        // Act
        await sut.DispatchAsync(
            context,
            [new FundingChangedDomainEvent(
                RolloverSourceTypes.Ofqual,
                newVersionId,
                fundingOfferId,
                oldVersionId)],
            CancellationToken);

        // Assert
        oldCandidate.IsActive.ShouldBeFalse();
        oldCandidate.SourceQualificationId.ShouldBe(oldVersionId);
        targetCandidate.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task SaveChangesAsync_WhenFundingChanges_DispatchesFundingEventFromContextBoundary()
    {
        // Arrange
        var dispatcher = new Mock<IFundingDomainEventDispatcher>();
        dispatcher
            .Setup(instance => instance.DispatchAsync(
                It.IsAny<ApplicationDbContext>(),
                It.IsAny<IReadOnlyCollection<FundingDomainEvent>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new ApplicationDbContext(options, dispatcher.Object);
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        context.QualificationFundings.Add(QualificationFundings.Create(
            qualificationVersionId,
            fundingOfferId,
            null,
            new DateOnly(2026, 7, 31)));

        // Act
        await context.SaveChangesAsync(CancellationToken);

        // Assert
        dispatcher.Verify(instance => instance.DispatchAsync(
            context,
            It.Is<IReadOnlyCollection<FundingDomainEvent>>(events =>
                events.OfType<FundingChangedDomainEvent>().Any(domainEvent =>
                    domainEvent.SourceType == RolloverSourceTypes.Ofqual &&
                    domainEvent.SourceQualificationId == qualificationVersionId &&
                    domainEvent.FundingOfferId == fundingOfferId)),
            CancellationToken), Times.Once);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenReconciliationFails_RollsBackFundingChange()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(CancellationToken);
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        var dispatcher = new Mock<IFundingDomainEventDispatcher>();
        dispatcher
            .Setup(instance => instance.DispatchAsync(
                It.IsAny<ApplicationDbContext>(),
                It.IsAny<IReadOnlyCollection<FundingDomainEvent>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Reconciliation failed."));
        await using var context = new ApplicationDbContext(options, dispatcher.Object);
        await context.Database.EnsureCreatedAsync(CancellationToken);
        await context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF", CancellationToken);
        context.QualificationFundings.Add(QualificationFundings.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            new DateOnly(2026, 7, 31)));

        // Act
        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            context.SaveChangesAsync(CancellationToken));

        // Assert
        exception.Message.ShouldBe("Reconciliation failed.");
        context.ChangeTracker.Clear();
        (await context.QualificationFundings.CountAsync(CancellationToken)).ShouldBe(0);
    }

    private static FundingDomainEventDispatcher CreateDispatcher()
    {
        var clock = new Mock<ISystemClockProvider>();
        clock.SetupGet(provider => provider.UtcNow).Returns(Now);
        return new FundingDomainEventDispatcher(
            clock.Object,
            NullLogger<FundingDomainEventDispatcher>.Instance);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
