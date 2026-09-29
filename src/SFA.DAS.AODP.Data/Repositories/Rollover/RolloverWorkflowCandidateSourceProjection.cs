namespace SFA.DAS.AODP.Data.Repositories.Rollover;

internal class RolloverWorkflowCandidateSourceProjection
{
    public Guid WorkflowCandidateId { get; set; }

    public string SourceType { get; set; } = null!;

    public Guid SourceQualificationId { get; set; }

    public Guid FundingOfferId { get; set; }

    public string FundingStreamName { get; set; } = null!;

    public string QualificationReference { get; set; } = null!;

    public string QualificationTitle { get; set; } = string.Empty;

    public string AwardingOrganisation { get; set; } = string.Empty;

    public string QualificationLevel { get; set; } = string.Empty;

    public string QualificationType { get; set; } = string.Empty;

    public string SSA { get; set; } = string.Empty;

    public DateTime? OperationalEndDate { get; set; }

    public bool OfferedInEngland { get; set; }

    public bool FundedInEngland { get; set; }

    public int? GLH { get; set; }

    public int? TQT { get; set; }

    public bool Pre16 { get; set; }

    public bool Age16To18 { get; set; }

    public bool Age18Plus { get; set; }

    public bool Age19Plus { get; set; }

    public DateOnly? FundingApprovalStartDate { get; set; }

    public bool PassP1 { get; set; }

    public string? ExclusionReason { get; set; }

    public string? P1FailureReason { get; set; }

    public DateTime CurrentFundingEndDate { get; set; }

    public DateTime? ProposedFundingEndDate { get; set; }
}
