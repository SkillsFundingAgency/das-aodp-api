using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverCandidateReconciler
{
    Task<FundingReconciliationResult> ReconcileAsync(
        IReadOnlyCollection<FundingChangeKey> keys,
        CancellationToken cancellationToken);
}

public sealed record FundingReconciliationResult(
    int Created,
    int Refreshed,
    int Deactivated,
    int Reactivated,
    int WorkflowsInvalidated,
    IReadOnlyCollection<FundingReconciliationOutcome> Outcomes);

public sealed record FundingReconciliationOutcome(
    FundingChangeKey Key,
    string Outcome,
    int AffectedCount);
