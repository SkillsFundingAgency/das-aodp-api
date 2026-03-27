using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverRepository
{
    Task<int> GetRolloverWorkflowCandidatesCountAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Entities.Rollover.RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverCandidate>> GetRolloverCandidatesAsync(CancellationToken cancellationToken);
    Task<RolloverWorkflowRun> GeRolloverWorkflowRunByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<RolloverCandidate>> GetRolloverCandidatesByIdsAsync(IReadOnlyCollection<Guid> rolloverCandidateIds, 
        CancellationToken cancellationToken);
    Task<Guid> CreateRolloverWorkflowRunAsync(RolloverWorkflowRun request,
        CancellationToken cancellationToken);
    Task CreateRolloverWorkflowCandidatesAsync(IEnumerable<Entities.Rollover.RolloverWorkflowCandidate> request,
        CancellationToken cancellationToken);
    Task CreateRolloverWorkflowRunFundingOffersAsync(IEnumerable<RolloverWorkflowRunFundingOffer> request,
        CancellationToken cancellationToken);
}