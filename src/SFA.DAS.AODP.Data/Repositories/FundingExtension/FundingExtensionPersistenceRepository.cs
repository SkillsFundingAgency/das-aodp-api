using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.FundingExtension;

public class FundingExtensionPersistenceRepository(
    ApplicationDbContext context,
    ILogger<FundingExtensionPersistenceRepository> logger)
    : IFundingExtensionPersistenceRepository
{
    private const int BatchSize = 2_000;

    public async Task PersistAsync(
        IReadOnlyCollection<RolloverCandidates> candidates,
        IReadOnlyCollection<QualificationFundings> fundings,
        IReadOnlyCollection<QualificationDiscussionHistory> histories,
        CancellationToken cancellationToken)
    {
        var totalStarted = Stopwatch.GetTimestamp();
        var transactionStarted = Stopwatch.GetTimestamp();
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        logger.LogInformation(
            "Started funding-extension persistence transaction in {ElapsedMilliseconds} ms",
            Stopwatch.GetElapsedTime(transactionStarted).TotalMilliseconds);
        
        var currentStage = "CandidateBulkUpdate";

        try
        {
            if (candidates.Count > 0)
            {
                var candidateUpdateStarted = Stopwatch.GetTimestamp();
                var candidateConfig = new BulkConfig
                {
                    BatchSize = BatchSize,
                    PropertiesToInclude =
                    [
                        nameof(RolloverCandidates.RolloverStatus),
                        nameof(RolloverCandidates.ExclusionReason),
                        nameof(RolloverCandidates.NewFundingEndDate)
                    ],
                    UseTempDB = true
                };

                await context.BulkUpdateAsync(
                    candidates.ToList(),
                    candidateConfig,
                    cancellationToken: cancellationToken);
                
                logger.LogInformation(
                    "Bulk updated {CandidateCount} rollover candidates in {ElapsedMilliseconds} ms using batch size {BatchSize}",
                    candidates.Count,
                    Stopwatch.GetElapsedTime(candidateUpdateStarted).TotalMilliseconds,
                    BatchSize);
            }

            currentStage = "FundingBulkUpdate";
            if (fundings.Count > 0)
            {
                var fundingUpdateStarted = Stopwatch.GetTimestamp();
                var fundingConfig = new BulkConfig
                {
                    BatchSize = BatchSize,
                    PropertiesToInclude =
                    [
                        nameof(QualificationFundings.EndDate),
                        nameof(QualificationFundings.Comments)
                    ],
                    UseTempDB = true
                };

                await context.BulkUpdateAsync(
                    fundings.ToList(),
                    fundingConfig,
                    cancellationToken: cancellationToken);
                
                logger.LogInformation(
                    "Bulk updated {FundingCount} qualification fundings in {ElapsedMilliseconds} ms using batch size {BatchSize}",
                    fundings.Count,
                    Stopwatch.GetElapsedTime(fundingUpdateStarted).TotalMilliseconds,
                    BatchSize);
            }

            currentStage = "HistoryBulkInsert";
            if (histories.Count > 0)
            {
                var historyInsertStarted = Stopwatch.GetTimestamp();
                await context.BulkInsertAsync(
                    histories.ToList(),
                    new BulkConfig { BatchSize = BatchSize, UseTempDB = true },
                    cancellationToken: cancellationToken);
                
                logger.LogInformation(
                    "Bulk inserted {HistoryCount} discussion-history records in {ElapsedMilliseconds} ms using batch size {BatchSize}",
                    histories.Count,
                    Stopwatch.GetElapsedTime(historyInsertStarted).TotalMilliseconds,
                    BatchSize);
            }

            currentStage = "WorkflowCandidateDelete";
            var workflowDeleteStarted = Stopwatch.GetTimestamp();
            await context.RolloverWorkflowCandidates.ExecuteDeleteAsync(cancellationToken);
            
            logger.LogInformation(
                "Deleted rollover workflow candidates in {ElapsedMilliseconds} ms",
                Stopwatch.GetElapsedTime(workflowDeleteStarted).TotalMilliseconds);

            currentStage = "TransactionCommit";
            var commitStarted = Stopwatch.GetTimestamp();
            await transaction.CommitAsync(cancellationToken);
            
            logger.LogInformation(
                "Committed funding-extension persistence transaction in {ElapsedMilliseconds} ms; total persistence time was {TotalElapsedMilliseconds} ms",
                Stopwatch.GetElapsedTime(commitStarted).TotalMilliseconds,
                Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Funding-extension persistence failed during {PersistenceStage} after {ElapsedMilliseconds} ms",
                currentStage,
                Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
            
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
