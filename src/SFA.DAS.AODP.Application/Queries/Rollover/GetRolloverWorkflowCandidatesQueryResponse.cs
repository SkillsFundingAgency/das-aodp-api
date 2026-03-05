using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetRolloverWorkflowCandidatesQueryResponse
{
    public List<RolloverWorkflowCandidate> Data { get; set; } = new();
    public int? Skip { get; set; }
    public int? Take { get; set; }
    public int TotalRecords { get; set; }
}
