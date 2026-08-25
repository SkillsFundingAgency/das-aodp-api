using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Funding;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Entities.Funding;

public class FundingDomainEventCollectorTests : UnitTest
{
    [Fact]
    public async Task Collect_WhenTrackedPropertyChangedDirectlyWithoutDomainMethod_RaisesFundingChangedEvent()
    {
        // Arrange - directly mutating EndDate bypasses UpdateFunding()/RecordChanged(), so the
        // only thing that can catch this is AddFundingChange's own change-tracker inspection.
        await using var context = CreateContext();
        var funding = QualificationFundings.Create(Guid.NewGuid(), Guid.NewGuid(), null, null);
        context.QualificationFundings.Add(funding);
        await context.SaveChangesAsync(CancellationToken);
        FundingDomainEventCollector.Clear(context.ChangeTracker);

        var untouched = QualificationFundings.Create(Guid.NewGuid(), Guid.NewGuid(), null, null);
        context.QualificationFundings.Add(untouched);
        await context.SaveChangesAsync(CancellationToken);
        FundingDomainEventCollector.Clear(context.ChangeTracker);

        funding.EndDate = new DateOnly(2027, 7, 31);

        // Act
        var events = FundingDomainEventCollector.Collect(context.ChangeTracker);

        // Assert - only the entity that actually changed raises an event
        var changeEvent = events.OfType<FundingChangedDomainEvent>().ShouldHaveSingleItem();
        changeEvent.SourceType.ShouldBe(RolloverSourceTypes.Ofqual);
        changeEvent.SourceQualificationId.ShouldBe(funding.QualificationVersionId);
        changeEvent.FundingOfferId.ShouldBe(funding.FundingOfferId);
        changeEvent.PreviousSourceQualificationId.ShouldBeNull();
    }

    [Fact]
    public async Task Collect_WhenSourceQualificationIdChangedDirectly_RaisesEventWithPreviousId()
    {
        // Arrange
        await using var context = CreateContext();
        var originalVersionId = Guid.NewGuid();
        var newVersionId = Guid.NewGuid();
        var funding = QualificationFundings.Create(originalVersionId, Guid.NewGuid(), null, null);
        context.QualificationFundings.Add(funding);
        await context.SaveChangesAsync(CancellationToken);
        FundingDomainEventCollector.Clear(context.ChangeTracker);

        funding.QualificationVersionId = newVersionId;

        // Act
        var events = FundingDomainEventCollector.Collect(context.ChangeTracker);

        // Assert
        var changeEvent = events.OfType<FundingChangedDomainEvent>().ShouldHaveSingleItem();
        changeEvent.SourceQualificationId.ShouldBe(newVersionId);
        changeEvent.PreviousSourceQualificationId.ShouldBe(originalVersionId);
    }

    [Fact]
    public async Task Collect_WhenTrackedEntityIsDeleted_ThrowsBecauseRecordsMustBeArchivedNotDeleted()
    {
        // Arrange
        await using var context = CreateContext();
        var funding = QualificationFundings.Create(Guid.NewGuid(), Guid.NewGuid(), null, null);
        context.QualificationFundings.Add(funding);
        await context.SaveChangesAsync(CancellationToken);
        context.QualificationFundings.Remove(funding);

        // Act / Assert
        var exception = Should.Throw<InvalidOperationException>(() =>
            FundingDomainEventCollector.Collect(context.ChangeTracker));
        exception.Message.ShouldContain(nameof(QualificationFundings));
    }

    [Fact]
    public async Task Collect_WhenEligibleForFundingChanges_RaisesEligibilityChangedEvent()
    {
        // Arrange
        await using var context = CreateContext();
        var version = new QualificationVersions
        {
            Id = Guid.NewGuid(),
            QualificationId = Guid.NewGuid(),
            Version = 1,
            EligibleForFunding = true,
            Status = "Available",
            Type = "Test",
            Ssa = "Test",
            Level = "3",
            SubLevel = "3",
            EqfLevel = "4",
            LastUpdatedDate = new DateTime(2026, 1, 1),
            InsertedDate = new DateTime(2026, 1, 1)
        };
        context.QualificationVersions.Add(version);
        await context.SaveChangesAsync(CancellationToken);

        version.EligibleForFunding = false;

        // Act
        var events = FundingDomainEventCollector.Collect(context.ChangeTracker);

        // Assert
        var eligibilityEvent = events.OfType<QualificationFundingEligibilityChangedDomainEvent>().ShouldHaveSingleItem();
        eligibilityEvent.SourceType.ShouldBe(RolloverSourceTypes.Ofqual);
        eligibilityEvent.SourceQualificationId.ShouldBe(version.Id);
    }

    [Fact]
    public async Task Clear_RemovesRecordedEventsFromTrackedEntities()
    {
        // Arrange
        await using var context = CreateContext();
        var funding = QualificationFundings.Create(Guid.NewGuid(), Guid.NewGuid(), null, null);
        context.QualificationFundings.Add(funding);
        funding.FundingDomainEvents.ShouldNotBeEmpty();

        // Act
        FundingDomainEventCollector.Clear(context.ChangeTracker);

        // Assert
        funding.FundingDomainEvents.ShouldBeEmpty();
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"FundingDomainEventCollector_{Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }
}
