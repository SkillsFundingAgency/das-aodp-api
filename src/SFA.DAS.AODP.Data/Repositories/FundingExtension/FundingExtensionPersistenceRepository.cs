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
        var operationId = Guid.NewGuid();
        var totalStarted = Stopwatch.GetTimestamp();
        var currentStage = "StartTransaction";

        using var logScope = logger.BeginScope(
            new Dictionary<string, object> { ["FundingExtensionOperationId"] = operationId });

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var createdAt = DateTime.UtcNow;
            var stagingRows = CreateStagingRows(
                operationId,
                candidates,
                fundings,
                createdAt);

            currentStage = "StageChanges";
            var stagingStarted = Stopwatch.GetTimestamp();
            if (stagingRows.Count > 0)
            {
                await context.BulkInsertAsync(
                    stagingRows,
                    new BulkConfig
                    {
                        BatchSize = BatchSize,
                        SetOutputIdentity = false
                    },
                    cancellationToken: cancellationToken);
            }

            logger.LogInformation(
                "Staged {StagingRowCount} funding-extension rows in {ElapsedMilliseconds} ms",
                stagingRows.Count,
                Stopwatch.GetElapsedTime(stagingStarted).TotalMilliseconds);

            var operationRows = context.FundingExtensionStaging
                .Where(row => row.OperationId == operationId);

            currentStage = "ApplyCandidateUpdates";
            var candidateUpdateStarted = Stopwatch.GetTimestamp();
            var updatedCandidateCount = await context.RolloverCandidates
                .Where(candidate => operationRows.Any(
                    row => row.RolloverCandidateId == candidate.Id))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        candidate => candidate.RolloverStatus,
                        candidate => operationRows
                            .Where(row => row.RolloverCandidateId == candidate.Id)
                            .Select(row => row.RolloverStatus)
                            .First())
                    .SetProperty(
                        candidate => candidate.ExclusionReason,
                        candidate => operationRows
                            .Where(row => row.RolloverCandidateId == candidate.Id)
                            .Select(row => row.ExclusionReason)
                            .First())
                    .SetProperty(
                        candidate => candidate.NewFundingEndDate,
                        candidate => operationRows
                            .Where(row => row.RolloverCandidateId == candidate.Id)
                            .Select(row => row.NewFundingEndDate)
                            .First()),
                    cancellationToken);

            EnsureExpectedRowCount("rollover candidate", candidates.Count, updatedCandidateCount);

            logger.LogInformation(
                "Updated {CandidateCount} rollover candidates in {ElapsedMilliseconds} ms",
                updatedCandidateCount,
                Stopwatch.GetElapsedTime(candidateUpdateStarted).TotalMilliseconds);

            currentStage = "ApplyFundingUpdates";
            var fundingUpdateStarted = Stopwatch.GetTimestamp();
            var updatedFundingCount = await context.QualificationFundings
                .Where(funding => operationRows.Any(
                    row => row.QualificationFundingId == funding.Id))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        funding => funding.EndDate,
                        funding => operationRows
                            .Where(row => row.QualificationFundingId == funding.Id)
                            .Select(row => row.FundingEndDate)
                            .First())
                    .SetProperty(
                        funding => funding.Comments,
                        funding => operationRows
                            .Where(row => row.QualificationFundingId == funding.Id)
                            .Select(row => row.FundingComments)
                            .First()),
                    cancellationToken);

            EnsureExpectedRowCount("qualification funding", fundings.Count, updatedFundingCount);

            logger.LogInformation(
                "Updated {FundingCount} qualification fundings in {ElapsedMilliseconds} ms",
                updatedFundingCount,
                Stopwatch.GetElapsedTime(fundingUpdateStarted).TotalMilliseconds);

            currentStage = "InsertHistory";
            var historyStarted = Stopwatch.GetTimestamp();
            if (histories.Count > 0)
            {
                await context.BulkInsertAsync(
                    histories.ToList(),
                    new BulkConfig
                    {
                        BatchSize = BatchSize,
                        SetOutputIdentity = false
                    },
                    cancellationToken: cancellationToken);
            }

            logger.LogInformation(
                "Inserted {HistoryCount} discussion-history rows in {ElapsedMilliseconds} ms",
                histories.Count,
                Stopwatch.GetElapsedTime(historyStarted).TotalMilliseconds);

            currentStage = "CompleteWorkflow";
            await context.RolloverWorkflowCandidates.ExecuteDeleteAsync(cancellationToken);

            currentStage = "ClearStaging";
            var clearedStagingCount = await context.FundingExtensionStaging
                .Where(row => row.OperationId == operationId)
                .ExecuteDeleteAsync(cancellationToken);
            EnsureExpectedRowCount("funding-extension staging", stagingRows.Count, clearedStagingCount);

            currentStage = "CommitTransaction";
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Completed funding-extension persistence for {CandidateCount} candidates and cleared {StagingRowCount} staging rows in {ElapsedMilliseconds} ms",
                candidates.Count,
                stagingRows.Count,
                Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Funding-extension persistence failed during {PersistenceStage} after {ElapsedMilliseconds} ms",
                currentStage,
                Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);

            await RollbackAndCleanupAsync(transaction, operationId);
            throw;
        }
        finally
        {
            context.ChangeTracker.Clear();
        }
    }

    private static List<FundingExtensionStaging> CreateStagingRows(
        Guid operationId,
        IReadOnlyCollection<RolloverCandidates> candidates,
        IReadOnlyCollection<QualificationFundings> fundings,
        DateTime createdAt)
    {
        var fundingLookup = fundings.ToDictionary(
            funding => (funding.QualificationVersionId, funding.FundingOfferId));

        return candidates.Select(candidate =>
        {
            fundingLookup.TryGetValue(
                (candidate.QualificationVersionId, candidate.FundingOfferId),
                out var funding);

            return new FundingExtensionStaging
            {
                OperationId = operationId,
                RolloverCandidateId = candidate.Id,
                QualificationFundingId = funding?.Id,
                RolloverStatus = candidate.RolloverStatus,
                ExclusionReason = candidate.ExclusionReason,
                NewFundingEndDate = candidate.NewFundingEndDate,
                FundingEndDate = funding?.EndDate,
                FundingComments = funding?.Comments,
                CreatedAt = createdAt
            };
        }).ToList();
    }

    private static void EnsureExpectedRowCount(
        string rowType,
        int expectedCount,
        int actualCount)
    {
        if (expectedCount != actualCount)
        {
            throw new DbUpdateConcurrencyException(
                $"Expected to affect {expectedCount} {rowType} rows but affected {actualCount}.");
        }
    }

    private async Task RollbackAndCleanupAsync(
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction,
        Guid operationId)
    {
        try
        {
            await transaction.RollbackAsync(CancellationToken.None);
        }
        catch (Exception rollbackException)
        {
            logger.LogError(
                rollbackException,
                "Failed to roll back funding-extension transaction");
        }

        try
        {
            var cleanedStagingCount = await context.FundingExtensionStaging
                .Where(row => row.OperationId == operationId)
                .ExecuteDeleteAsync(CancellationToken.None);

            if (cleanedStagingCount > 0)
            {
                logger.LogWarning(
                    "Removed {StagingRowCount} funding-extension staging rows after failure",
                    cleanedStagingCount);
            }
        }
        catch (Exception cleanupException)
        {
            logger.LogError(
                cleanupException,
                "Failed to clean funding-extension staging rows after rollback");
        }
    }
}
