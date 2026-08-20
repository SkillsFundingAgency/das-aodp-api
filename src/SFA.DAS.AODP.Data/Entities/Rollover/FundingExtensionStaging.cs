using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[Table("FundingExtensionStaging")]
[ExcludeFromCodeCoverage]
public class FundingExtensionStaging
{
    public Guid OperationId { get; set; }
    public Guid RolloverCandidateId { get; set; }
    public string? SourceType { get; set; }
    public Guid? SourceFundingRecordId { get; set; }
    public RolloverStatus RolloverStatus { get; set; }
    public string? ExclusionReason { get; set; }
    public DateTime? NewFundingEndDate { get; set; }
    public DateOnly? FundingEndDate { get; set; }
    public string? FundingComments { get; set; }
    public DateTime CreatedAt { get; set; }
}

