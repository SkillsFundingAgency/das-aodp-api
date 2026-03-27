using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover
{
    public class GetRolloverWorkflowCandidatesQueryResponse
    {
        public Guid WorkflowRunId { get; set; }
        public DateTime? FundingEndDateEligibilityThreshold { get; set; }
        public DateTime? OperationalEndDateEligibilityThreshold { get; set; }
        public DateTime? MaximumApprovalFundingEndDate { get; set; }
        public IEnumerable<RolloverWorkflowCandidate> RolloverWorkflowCandidates { get; set; } = new List<RolloverWorkflowCandidate>();
    }
}