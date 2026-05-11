using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverRepository
{
    Task<int> GetRolloverWorkflowCandidatesCountAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverWorkflowCandidatesP1Checks>> GetRolloverWorkflowCandidatesP1ChecksAsync(CancellationToken cancellationToken);
    Task UpdateRolloverWorkflowCandidatesAsync(IEnumerable<RolloverWorkflowCandidate> candidates, CancellationToken cancellationToken);
}
