using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public class FundingChangeCoordinator(
    IApplicationDbContext context,
    IRolloverCandidateReconciler reconciler,
    ILogger<FundingChangeCoordinator> logger)
    : IFundingChangeCoordinator
{
    public async Task<TResult> ExecuteAsync<TResult>(
        FundingChangeSet changeSet,
        Func<CancellationToken, Task<TResult>> applyFundingMutation,
        CancellationToken cancellationToken)
    {
        await using var transaction = await context.StartTransactionAsync();
        TResult result;
        FundingReconciliationResult reconciliation;

        try
        {
            result = await applyFundingMutation(cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            reconciliation = await reconciler.ReconcileAsync(
                changeSet.Keys,
                cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        foreach (var outcome in reconciliation.Outcomes)
        {
            logger.LogInformation(
                "Funding change reconciled. SourceType: {SourceType}, SourceQualificationId: {SourceQualificationId}, FundingOfferId: {FundingOfferId}, AcademicYear: {AcademicYear}, Outcome: {Outcome}, AffectedCount: {AffectedCount}.",
                outcome.Key.SourceType,
                outcome.Key.SourceQualificationId,
                outcome.Key.FundingOfferId,
                outcome.Key.AcademicYear,
                outcome.Outcome,
                outcome.AffectedCount);
        }

        logger.LogInformation(
            "Reconciled {FundingChangeCount} funding changes. Created: {Created}, refreshed: {Refreshed}, deactivated: {Deactivated}, reactivated: {Reactivated}, workflows invalidated: {WorkflowsInvalidated}.",
            changeSet.Keys.Count,
            reconciliation.Created,
            reconciliation.Refreshed,
            reconciliation.Deactivated,
            reconciliation.Reactivated,
            reconciliation.WorkflowsInvalidated);

        return result;
    }
}
