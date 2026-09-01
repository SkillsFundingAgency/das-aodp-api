using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;
namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverRepository
{
    Task<int> GetRolloverWorkflowCandidatesCountAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverWorkflowCandidate>> GetAllRolloverWorkflowCandidatesAsync(CancellationToken cancellationToken);

    Task UpdateRolloverWorkflowCandidatesAsync(IEnumerable<RolloverWorkflowCandidate> candidates, CancellationToken cancellationToken);

    Task<IEnumerable<RolloverCandidateDto>> GetRolloverCandidatesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RolloverCandidateDto>> GetQualificationVersionsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderRequest filters,
        CancellationToken cancellationToken);

    Task<IEnumerable<RolloverCandidateDto>> GetRolloverCandidatesByIdsAsync(IReadOnlyCollection<Guid> rolloverCandidateIds, 
        CancellationToken cancellationToken);
    Task<IReadOnlyList<RolloverCandidateP1CheckData>> GetRolloverCandidatesWithP1ChecksAsync(
        IReadOnlyCollection<RolloverCandidateP1CheckRequest> requests,
        CancellationToken cancellationToken);
    Task<RolloverWorkflowRun?> GeRolloverWorkflowRunByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Guid> CreateRolloverWorkflowAsync(
        RolloverWorkflowRun workflowRun,
        IReadOnlyCollection<RolloverWorkflowCandidate> workflowCandidates,
        IReadOnlyCollection<RolloverWorkflowRunFundingOffer> workflowFundingOffers,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<RolloverCandidateForExport>> GetRolloverWorkflowCandidatesByRunId(Guid workflowRunId, CancellationToken cancellationToken);

    Task<FundingExtensionCandidateValidationContext> GetFundingExtensionValidationContextAsync(
        HashSet<CandidateKey> incomingCandidates,
        CancellationToken cancellationToken);

    Task<List<RolloverCandidateStatusItem>> GetRolloverCandidatesStatusAsync(CancellationToken cancellationToken);

    Task<List<RolloverCandidates>> LoadRolloverCandidateGraphAsync(
        List<CandidateKey> keys,
        CancellationToken cancellationToken);

    Task DeleteAllWorkflowCandidatesAsync(CancellationToken cancellationToken);

    Task<Guid?> GetLatestWorkflowRunIdAsync(CancellationToken cancellationToken);

    Task<IEnumerable<RolloverQueryBuilderLevel>> GetAllLevelsForRolloverQueryBuilderAsync(CancellationToken cancellationToken);

    Task<IEnumerable<RolloverQueryBuilderSectorSubjectArea>> GetSectorSubjectAreasForRolloverQueryBuilderAsync(
        RolloverQueryBuilderSectorSubjectAreaRequest requestFilters, 
        CancellationToken cancellationToken);
    
    Task<IEnumerable<RolloverQueryBuilderType>> GetTypesForRolloverQueryBuilderAsync(
        RolloverQueryBuilderTypesRequest requestFilters, 
        CancellationToken cancellationToken);

    Task<IEnumerable<RolloverQueryBuilderAwardingOrganisation>> GetAwardingOrganisationsForRolloverQueryBuilderAsync(
        RolloverQueryBuilderAwardingOrganisationsRequest filters,
        CancellationToken cancellationToken);
    Task<RolloverStartSummary> GetRolloverStartSummaryAsync(string academicYear, CancellationToken cancellationToken);
}
