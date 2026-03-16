using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Rollover;

[ExcludeFromCodeCoverage]
public class RolloverWorkflowCandidateDto
{
    public Guid Id { get; set; }
    public Guid RolloverWorkflowRunId { get; set; }
    public Guid QualificationVersionId { get; set; }
    public Guid FundingOfferId { get; set; }
    public string AcademicYear { get; set; } = null!;
    public Guid RolloverCandidatesId { get; set; }
    public bool PassP1 { get; set; }
    public string? P1FailureReason { get; set; }
    public bool IncludedInP1Export { get; set; }
    public bool IncludedInFinalUpload { get; set; }
    public DateTime CurrentFundingEndDate { get; set; }
    public DateTime? ProposedFundingEndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
