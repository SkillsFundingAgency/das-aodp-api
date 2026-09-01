using SFA.DAS.AODP.Data.Entities.Funding;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Data.UnitTests.Entities.QaaQualification;

public class QaaQualificationFundingTests : UnitTest
{
    private static readonly DateTime CreatedAt = new(2026, 8, 1, 9, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_SetsFieldsAndRecordsFundingChangedEvent()
    {
        var qaaQualificationId = Guid.NewGuid();
        var fundingOfferId = Guid.NewGuid();

        var funding = QaaQualificationFunding.Create(
            qaaQualificationId,
            fundingOfferId,
            new DateOnly(2026, 8, 1),
            new DateOnly(2027, 7, 31),
            "Approved",
            CreatedAt,
            "Initial comment");

        funding.QaaQualificationId.ShouldBe(qaaQualificationId);
        funding.FundingOfferId.ShouldBe(fundingOfferId);
        funding.StartDate.ShouldBe(new DateOnly(2026, 8, 1));
        funding.EndDate.ShouldBe(new DateOnly(2027, 7, 31));
        funding.FundingStatus.ShouldBe("Approved");
        funding.Comments.ShouldBe("Initial comment");
        funding.CreatedAt.ShouldBe(CreatedAt);
        funding.UpdatedAt.ShouldBe(CreatedAt);

        var domainEvent = funding.FundingDomainEvents.ShouldHaveSingleItem().ShouldBeOfType<FundingChangedDomainEvent>();
        domainEvent.SourceType.ShouldBe(RolloverSourceTypes.Qaa);
        domainEvent.SourceQualificationId.ShouldBe(qaaQualificationId);
        domainEvent.FundingOfferId.ShouldBe(fundingOfferId);
    }

    [Fact]
    public void Create_WhenQaaQualificationIdIsEmpty_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() =>
            QaaQualificationFunding.Create(Guid.Empty, Guid.NewGuid(), null, null, "Approved", CreatedAt));

        exception.ParamName.ShouldBe("qaaQualificationId");
    }

    [Fact]
    public void Create_WhenFundingOfferIdIsEmpty_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() =>
            QaaQualificationFunding.Create(Guid.NewGuid(), Guid.Empty, null, null, "Approved", CreatedAt));

        exception.ParamName.ShouldBe("fundingOfferId");
    }

    [Fact]
    public void Update_UpdatesFieldsAndRecordsAnotherFundingChangedEvent()
    {
        var funding = QaaQualificationFunding.Create(
            Guid.NewGuid(), Guid.NewGuid(), null, null, "Approved", CreatedAt);
        funding.ClearFundingDomainEvents();
        var updatedAt = CreatedAt.AddDays(30);

        funding.Update(new DateOnly(2026, 8, 1), new DateOnly(2028, 7, 31), "Extended", updatedAt, "Comments");

        funding.StartDate.ShouldBe(new DateOnly(2026, 8, 1));
        funding.EndDate.ShouldBe(new DateOnly(2028, 7, 31));
        funding.FundingStatus.ShouldBe("Extended");
        funding.Comments.ShouldBe("Comments");
        funding.UpdatedAt.ShouldBe(updatedAt);
        funding.FundingDomainEvents.ShouldHaveSingleItem();
    }

    [Fact]
    public void Archive_SetsEndDateAndRecordsFundingChangedEvent()
    {
        var funding = QaaQualificationFunding.Create(
            Guid.NewGuid(), Guid.NewGuid(), null, null, "Approved", CreatedAt);
        funding.ClearFundingDomainEvents();
        var updatedAt = CreatedAt.AddDays(30);

        funding.Archive(new DateOnly(2027, 7, 31), updatedAt, "No longer offered");

        funding.EndDate.ShouldBe(new DateOnly(2027, 7, 31));
        funding.Comments.ShouldBe("No longer offered");
        funding.UpdatedAt.ShouldBe(updatedAt);
        funding.FundingDomainEvents.ShouldHaveSingleItem();
    }

    [Fact]
    public void ClearFundingDomainEvents_EmptiesTheRecordedEvents()
    {
        var funding = QaaQualificationFunding.Create(
            Guid.NewGuid(), Guid.NewGuid(), null, null, "Approved", CreatedAt);
        funding.FundingDomainEvents.ShouldNotBeEmpty();

        funding.ClearFundingDomainEvents();

        funding.FundingDomainEvents.ShouldBeEmpty();
    }
}
