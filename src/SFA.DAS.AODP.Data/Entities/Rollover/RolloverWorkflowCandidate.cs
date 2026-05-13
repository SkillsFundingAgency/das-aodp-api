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

    public void SetP1Result(bool pass, string? failureReason = null)
    {
        PassP1 = pass;
        P1FailureReason = pass ? null : failureReason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EvaluateP1Checks(RolloverWorkflowCandidatesP1Checks checks)
    {
        var failures = new List<string>();

        // 1) Is the Funding Stream included in the RollOver
        if (checks.FundingStream == null)
            failures.Add("Funding Stream out of scope for RollOver");

        // 2) Latest Funding Approval End Date >= Threshold Date
        if (checks.LatestFundingApprovalEndDate.HasValue && checks.LatestFundingApprovalEndDate.Value < checks.ThresholdDate)
            failures.Add("Funding Approval End Date is before the Threshold");

        // 3) Operating End Date > Threshold Date  (If Operating End Date = Null, this should Pass the check)
        if (checks.OperationalEndDate.HasValue && checks.OperationalEndDate.Value <= checks.ThresholdDate)
            failures.Add("Operating End Date is before the Threshold");

        // 4) Offered in England = TRUE
        if (!checks.OfferedInEngland)
            failures.Add("Not Offered in England");

        // 5) Intention to seek funding in England = TRUE
        if (!checks.IntentionToSeekFundingInEngland)
            failures.Add("Not Funded in England");

        // 6) GLH <= TQT
        if (checks.Glh > checks.Tqt)
            failures.Add("GLH > TQT");

        // 7) Does the Qualification appear in the Defunding (Defunded) List
        if (checks.IsOnDefundingList)
            failures.Add("Qualification is on Defunding (Defunded) List");

        SetP1Result(
            failures.Count == 0,
            failures.Count > 0
                ? string.Join("; ", failures)
                : null);
    }
}