using SFA.DAS.AODP.Models.Rollover;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Queries.Rollover
{
    [ExcludeFromCodeCoverage]
    public class GetRolloverWorkflowCandidatesQueryResponse
    {
        public Guid WorkflowRunId { get; set; }
        public DateTime? FundingEndDateEligibilityThreshold { get; set; }
        public DateTime? OperationalEndDateEligibilityThreshold { get; set; }
        public DateTime? MaximumApprovalFundingEndDate { get; set; }
        public IEnumerable<RolloverWorkflowCandidateDto> RolloverWorkflowCandidates { get; set; } = new List<RolloverWorkflowCandidateDto>();
    }
}