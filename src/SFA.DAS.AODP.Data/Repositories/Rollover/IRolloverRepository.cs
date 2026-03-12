using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverRepository
{
    Task<int> GetRolloverWorkflowCandidatesCountAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken);
}
