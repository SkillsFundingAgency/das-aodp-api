using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;
namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverRepository
{
    Task<int> GetRolloverWorkflowCandidatesCountAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverWorkflowCandidatesP1Checks>> GetRolloverWorkflowCandidatesP1ChecksAsync(CancellationToken cancellationToken);

    Task UpdateRolloverWorkflowCandidatesAsync(IEnumerable<RolloverWorkflowCandidate> candidates, CancellationToken cancellationToken);

    Task<IEnumerable<RolloverCandidateDto>> GetRolloverCandidatesAsync(CancellationToken cancellationToken);
    Task<RolloverWorkflowRun> GeRolloverWorkflowRunByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Models.Rollover.RolloverCandidateDto>> GetRolloverCandidatesByIdsAsync(IReadOnlyCollection<Guid> rolloverCandidateIds, 
        CancellationToken cancellationToken);
    Task<Guid> CreateRolloverWorkflowRunAsync(RolloverWorkflowRun request,
        CancellationToken cancellationToken);

    Task CreateRolloverWorkflowCandidatesAsync(IEnumerable<RolloverWorkflowCandidate> workflowCandidates,
        CancellationToken cancellationToken);
    Task CreateRolloverWorkflowRunFundingOffersAsync(IEnumerable<RolloverWorkflowRunFundingOffer> request,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<FundingExtensionCandidateDto>> GetRolloverWorkflowCandidatesByRunId(Guid workflowRunId, CancellationToken cancellationToken);

    Task<FundingExtensionCandidateValidationContext> GetFundingExtensionValidationContextAsync(
        HashSet<CandidateKey> incomingCandidates,
        CancellationToken cancellationToken);

    Task<List<FundingExtensionCandidateItem>> GetFundingExtensionCandidatesAsync(CancellationToken cancellationToken);

}