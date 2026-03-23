namespace SFA.DAS.AODP.Data.Entities.Rollover;

public class RolloverWorkflowCandidatesP1Checks
{
    public Guid WorkflowCandidateId { get; set; }
    public Guid QualificationVersionId { get; set; }
    public Guid FundingOfferId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public bool IncludedInP1Export { get; set; }
    public bool IncludedInFinalUpload { get; set; }
    public DateTime CurrentFundingEndDate { get; set; }
    public DateTime? ProposedFundingEndDate { get; set; }

    public string? FundingStream { get; set; }
    public int? RolloverRound { get; set; }
    public DateTime? ThresholdDate { get; set; }
    public DateTime? LatestFundingApprovalEndDate { get; set; }
    public DateTime? OperationalStartDate { get; set; }
    public DateTime? OperationalEndDate { get; set; }
    public bool OfferedInEngland { get; set; }
    public int? Glh { get; set; }
    public int? Tqt { get; set; }
    public bool IsOnDefundingList { get; set; }
}
