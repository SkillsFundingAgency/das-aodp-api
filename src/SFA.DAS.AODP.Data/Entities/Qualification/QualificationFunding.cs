using SFA.DAS.AODP.Data.Entities.Offer;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

[Table("QualificationFundings", Schema = "funded")]
public class QualificationFundings
{
    public Guid Id { get; set; }
    public Guid QualificationVersionId { get; set; }
    public Guid FundingOfferId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Comments { get; set; }

    public virtual QualificationVersions QualificationVersion { get; set; }
    public virtual FundingOffer FundingOffer { get; set; }
}
