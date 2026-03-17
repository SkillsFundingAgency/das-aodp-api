using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverRepository
{
    Task<int> GetRolloverWorkflowCandidatesCountAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverCandidate>> GetRolloverCandidatesAsync();
    Task<RolloverWorkflowRun> CreateRolloverWorkflowRunAsync(RolloverWorkflowRun request, CancellationToken cancellationToken = default);
    Task AddWorkflowCandidatesAsync(Guid workflowRunId, string academicYear, IReadOnlyCollection<Guid> rolloverCandidateIds, CancellationToken cancellationToken = default);
}