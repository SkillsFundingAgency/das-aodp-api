namespace SFA.DAS.AODP.Models.Rollover;

public class RolloverWorkflowCandidatesResult
{
    public List<RolloverWorkflowCandidate> Data { get; set; } = new();
    public int? Skip { get; set; }
    public int? Take { get; set; }
    public int TotalRecords { get; set; }
}
