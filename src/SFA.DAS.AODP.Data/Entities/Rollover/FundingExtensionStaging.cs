using System.ComponentModel.DataAnnotations.Schema;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[Table("FundingExtensionStaging")]
public class FundingExtensionStaging
{
    public Guid OperationId { get; set; }
    public Guid RolloverCandidateId { get; set; }
    public Guid? QualificationFundingId { get; set; }
    public RolloverStatus RolloverStatus { get; set; }
    public string? ExclusionReason { get; set; }
    public DateTime? NewFundingEndDate { get; set; }
    public DateOnly? FundingEndDate { get; set; }
    public string? FundingComments { get; set; }
    public DateTime CreatedAt { get; set; }
}
