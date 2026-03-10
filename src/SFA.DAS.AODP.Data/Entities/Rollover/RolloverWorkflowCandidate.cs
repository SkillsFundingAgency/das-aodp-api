using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

[Table("RolloverWorkflowCandidate")]
public class RolloverWorkflowCandidate
{
    public Guid Id { get; private set; }

    public Guid RolloverWorkflowRunId { get; private set; }

    public Guid QualificationVersionId { get; private set; }
    
    public Guid FundingOfferId { get; private set; }
    
    public string AcademicYear { get; private set; } = null!;
    
    public Guid RolloverCandidatesId { get; private set; }

    public bool PassP1 { get; private set; }

    public string? P1FailureReason { get; private set; }

    public bool IncludedInP1Export { get; private set; } = true;
    
    public bool IncludedInFinalUpload { get; private set; }

    public DateTime CurrentFundingEndDate { get; private set; }
    
    public DateTime? ProposedFundingEndDate { get; private set; }

    public DateTime CreatedAt { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }

    public virtual RolloverWorkflowRun RolloverWorkflowRun { get; private set; } = null!;

    public virtual RolloverCandidates RolloverCandidates { get; set; } = null!;

    public static RolloverWorkflowCandidate Create(
        Guid workflowRunId,
        Guid rolloverCandidateRecordId,
        Guid qualificationVersionId,
        Guid fundingOfferId,
        string academicYear,
        DateTime currentFundingEndDate,
        DateTime? proposedFundingEndDate,
        DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(academicYear))
        {
            throw new ArgumentNullException(nameof(academicYear));
        }

        return new RolloverWorkflowCandidate
        {
            RolloverWorkflowRunId = workflowRunId,
            RolloverCandidatesId = rolloverCandidateRecordId,
            QualificationVersionId = qualificationVersionId,
            FundingOfferId = fundingOfferId,
            AcademicYear = academicYear,
            CurrentFundingEndDate = currentFundingEndDate,
            ProposedFundingEndDate = proposedFundingEndDate,
            CreatedAt = createdAt,
            UpdatedAt = createdAt,
            IncludedInP1Export = true,
            IncludedInFinalUpload = false,
            PassP1 = false
        };
    }
}