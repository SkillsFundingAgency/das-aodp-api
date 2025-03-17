using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Qualification;

[Table("QualificationFundingFeedbacks", Schema = "funded")]
public class QualificationFundingFeedbacks
{
    public Guid Id { get; set; }
    public Guid QualificationVersionId { get; set; }
    public string? Status { get; set; }
    public string? Comments { get; set; }

    public virtual QualificationVersions QualificationVersion { get; set; }
}
