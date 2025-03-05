using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

[Table("QualificationOffers", Schema = "funded")]
public partial class QualificationOffer
{
    public int Id { get; set; }

    public int? QualificationId { get; set; }

    public string? Name { get; set; }

    public string? Notes { get; set; }

    public bool? FundingAvailable { get; set; }

    public DateTime? FundingApprovalStartDate { get; set; }

    public DateTime? FundingApprovalEndDate { get; set; }

    public virtual FundedQualification? Qualification { get; set; }
}
