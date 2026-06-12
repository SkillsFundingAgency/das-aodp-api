using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Rollover
{
    [ExcludeFromCodeCoverage]
    public class FundingExtensionCandidateDto
    {
        public int RowNumber { get; set; }
        public string QAN { get; set; } = string.Empty;
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
        public string FundingStreamName { get; set; } = string.Empty;
        public DateOnly? FundingApprovalStartDate { get; set; }
        public string ProposedOutcome { get; set; } = string.Empty;
        public string RolloverStatus { get; set; } = string.Empty; 
        public string? ExclusionReason { get; set; }
        public DateTime CurrentFundingApprovalEndDate { get; set; }
        public DateTime? ProposedFundingApprovalEndDate { get; set; }
        public string? Comments { get; set; }
    }

}
