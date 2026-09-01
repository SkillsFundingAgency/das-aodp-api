using SFA.DAS.AODP.Data.Entities.Funding;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.Rollover;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

[Table("QualificationFundings", Schema = "funded")]
public class QualificationFundings : IFundingDomainEventSource
{
    private readonly List<FundingDomainEvent> _fundingDomainEvents = [];

    public Guid Id { get; set; }
    public Guid QualificationVersionId { get; set; }
    public Guid FundingOfferId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Comments { get; set; }

    public virtual QualificationVersions QualificationVersion { get; set; }
    public virtual FundingOffer FundingOffer { get; set; }

    [NotMapped]
    public IReadOnlyCollection<FundingDomainEvent> FundingDomainEvents => _fundingDomainEvents;

    public static QualificationFundings Create(
        Guid qualificationVersionId,
        Guid fundingOfferId,
        DateOnly? startDate,
        DateOnly? endDate,
        string? comments = null)
    {
        var funding = new QualificationFundings
        {
            Id = Guid.NewGuid(),
            QualificationVersionId = qualificationVersionId,
            FundingOfferId = fundingOfferId,
            StartDate = startDate,
            EndDate = endDate,
            Comments = comments
        };
        funding.RecordChanged();
        return funding;
    }

    public void UpdateFunding(DateOnly? startDate, DateOnly? endDate, string? comments)
    {
        StartDate = startDate;
        EndDate = endDate;
        Comments = comments;
        RecordChanged();
    }

    public void Archive(DateOnly endDate, string? comments = null)
    {
        EndDate = endDate;
        Comments = comments;
        RecordChanged();
    }

    public void MoveToQualificationVersion(Guid qualificationVersionId)
    {
        var previousQualificationVersionId = QualificationVersionId;
        QualificationVersionId = qualificationVersionId;
        _fundingDomainEvents.Add(new FundingChangedDomainEvent(
            RolloverSourceTypes.Ofqual,
            qualificationVersionId,
            FundingOfferId,
            previousQualificationVersionId));
    }

    public void ClearFundingDomainEvents() => _fundingDomainEvents.Clear();

    private void RecordChanged()
    {
        _fundingDomainEvents.Add(new FundingChangedDomainEvent(
            RolloverSourceTypes.Ofqual,
            QualificationVersionId,
            FundingOfferId));
    }
}
