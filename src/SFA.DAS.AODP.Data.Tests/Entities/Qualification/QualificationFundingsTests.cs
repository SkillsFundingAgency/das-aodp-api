using SFA.DAS.AODP.Data.Entities.Funding;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Entities.Qualification;

public class QualificationFundingsTests : UnitTest
{
    [Fact]
    public void Create_SetsFieldsAndRecordsFundingChangedEvent()
    {
        var qualificationVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();

        var funding = QualificationFundings.Create(
            qualificationVersionId,
            fundingOfferId,
            new DateOnly(2026, 8, 1),
            new DateOnly(2027, 7, 31),
            "Initial comment");

        funding.QualificationVersionId.ShouldBe(qualificationVersionId);
        funding.FundingOfferId.ShouldBe(fundingOfferId);
        funding.StartDate.ShouldBe(new DateOnly(2026, 8, 1));
        funding.EndDate.ShouldBe(new DateOnly(2027, 7, 31));
        funding.Comments.ShouldBe("Initial comment");

        var domainEvent = funding.FundingDomainEvents.ShouldHaveSingleItem().ShouldBeOfType<FundingChangedDomainEvent>();
        domainEvent.SourceType.ShouldBe(RolloverSourceTypes.Ofqual);
        domainEvent.SourceQualificationId.ShouldBe(qualificationVersionId);
        domainEvent.FundingOfferId.ShouldBe(fundingOfferId);
        domainEvent.PreviousSourceQualificationId.ShouldBeNull();
    }

    [Fact]
    public void UpdateFunding_UpdatesFieldsAndRecordsAnotherFundingChangedEvent()
    {
        var funding = QualificationFundings.Create(Guid.NewGuid(), Guid.NewGuid(), null, null);
        funding.ClearFundingDomainEvents();

        funding.UpdateFunding(new DateOnly(2026, 8, 1), new DateOnly(2028, 7, 31), "Extended");

        funding.StartDate.ShouldBe(new DateOnly(2026, 8, 1));
        funding.EndDate.ShouldBe(new DateOnly(2028, 7, 31));
        funding.Comments.ShouldBe("Extended");
        funding.FundingDomainEvents.ShouldHaveSingleItem();
    }

    [Fact]
    public void Archive_SetsEndDateAndCommentsAndRecordsFundingChangedEvent()
    {
        var funding = QualificationFundings.Create(Guid.NewGuid(), Guid.NewGuid(), null, null);
        funding.ClearFundingDomainEvents();

        funding.Archive(new DateOnly(2027, 7, 31), "No longer offered");

        funding.EndDate.ShouldBe(new DateOnly(2027, 7, 31));
        funding.Comments.ShouldBe("No longer offered");
        funding.FundingDomainEvents.ShouldHaveSingleItem();
    }

    [Fact]
    public void MoveToQualificationVersion_UpdatesIdAndRecordsPreviousVersionOnTheEvent()
    {
        var previousVersionId = Guid.NewGuid();
        var newVersionId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();
        var funding = QualificationFundings.Create(previousVersionId, fundingOfferId, null, null);
        funding.ClearFundingDomainEvents();

        funding.MoveToQualificationVersion(newVersionId);

        funding.QualificationVersionId.ShouldBe(newVersionId);
        var domainEvent = funding.FundingDomainEvents.ShouldHaveSingleItem().ShouldBeOfType<FundingChangedDomainEvent>();
        domainEvent.SourceQualificationId.ShouldBe(newVersionId);
        domainEvent.FundingOfferId.ShouldBe(fundingOfferId);
        domainEvent.PreviousSourceQualificationId.ShouldBe(previousVersionId);
    }

    [Fact]
    public void ClearFundingDomainEvents_EmptiesTheRecordedEvents()
    {
        var funding = QualificationFundings.Create(Guid.NewGuid(), Guid.NewGuid(), null, null);
        funding.FundingDomainEvents.ShouldNotBeEmpty();

        funding.ClearFundingDomainEvents();

        funding.FundingDomainEvents.ShouldBeEmpty();
    }
}
