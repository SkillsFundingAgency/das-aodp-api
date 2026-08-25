using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.FundingExtension;

public class FundingExtensionPersistenceRepository(
    ApplicationDbContext context,
    ILogger<FundingExtensionPersistenceRepository> logger)
    : IFundingExtensionPersistenceRepository
{
    private const int BatchSize = 2_000;

    public async Task PersistAsync(
        IReadOnlyCollection<RolloverCandidates> candidates,
        IReadOnlyCollection<RolloverFundingUpdate> fundingUpdates,
        IReadOnlyCollection<QualificationDiscussionHistory> histories,
        IReadOnlyCollection<QaaQualificationDiscussionHistory> qaaHistories,
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
                fundingUpdates,
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

            // Funding tables are polymorphic by SourceType, so bulk updates are applied in two
            // set-based passes - one per underlying funding table - rather than one combined
            // pass, since a single staging row cannot point at two different tables.

            currentStage = "ApplyOfqualFundingUpdates";
            var ofqualFundingUpdateStarted = Stopwatch.GetTimestamp();
            var ofqualOperationRows = operationRows.Where(row => row.SourceType == RolloverSourceTypes.Ofqual);
            var updatedOfqualFundingCount = await context.QualificationFundings
                .Where(funding => ofqualOperationRows.Any(
                    row => row.SourceFundingRecordId == funding.Id))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        funding => funding.EndDate,
                        funding => ofqualOperationRows
                            .Where(row => row.SourceFundingRecordId == funding.Id)
                            .Select(row => row.FundingEndDate)
                            .First())
                    .SetProperty(
                        funding => funding.Comments,
                        funding => ofqualOperationRows
                            .Where(row => row.SourceFundingRecordId == funding.Id)
                            .Select(row => row.FundingComments)
                            .First()),
                    cancellationToken);

            logger.LogInformation(
                "Updated {FundingCount} Ofqual qualification fundings in {ElapsedMilliseconds} ms",
                updatedOfqualFundingCount,
                Stopwatch.GetElapsedTime(ofqualFundingUpdateStarted).TotalMilliseconds);

            currentStage = "ApplyQaaFundingUpdates";
            var qaaFundingUpdateStarted = Stopwatch.GetTimestamp();
            var qaaOperationRows = operationRows.Where(row => row.SourceType == RolloverSourceTypes.Qaa);
            var updatedQaaFundingCount = await context.QaaQualificationFundings
                .Where(funding => qaaOperationRows.Any(
                    row => row.SourceFundingRecordId == funding.Id))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(
                        funding => funding.EndDate,
                        funding => qaaOperationRows
                            .Where(row => row.SourceFundingRecordId == funding.Id)
                            .Select(row => row.FundingEndDate)
                            .First())
                    .SetProperty(
                        funding => funding.Comments,
                        funding => qaaOperationRows
                            .Where(row => row.SourceFundingRecordId == funding.Id)
                            .Select(row => row.FundingComments)
                            .First()),
                    cancellationToken);

            logger.LogInformation(
                "Updated {FundingCount} QAA qualification fundings in {ElapsedMilliseconds} ms",
                updatedQaaFundingCount,
                Stopwatch.GetElapsedTime(qaaFundingUpdateStarted).TotalMilliseconds);

            EnsureExpectedRowCount(
                "qualification funding",
                fundingUpdates.Count,
                updatedOfqualFundingCount + updatedQaaFundingCount);

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

            if (qaaHistories.Count > 0)
            {
                await context.BulkInsertAsync(
                    qaaHistories.ToList(),
                    new BulkConfig
                    {
                        BatchSize = BatchSize,
                        SetOutputIdentity = false
                    },
                    cancellationToken: cancellationToken);
            }

            logger.LogInformation(
                "Inserted {HistoryCount} discussion-history rows ({QaaHistoryCount} QAA) in {ElapsedMilliseconds} ms",
                histories.Count,
                qaaHistories.Count,
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

    internal static List<FundingExtensionStaging> CreateStagingRows(
        Guid operationId,
        IReadOnlyCollection<RolloverCandidates> candidates,
        IReadOnlyCollection<RolloverFundingUpdate> fundingUpdates,
        DateTime createdAt)
    {
        var fundingLookup = fundingUpdates.ToDictionary(
            funding => (funding.SourceType, funding.SourceQualificationId, funding.FundingOfferId));

        return candidates.Select(candidate =>
        {
            fundingLookup.TryGetValue(
                (candidate.SourceType, candidate.SourceQualificationId, candidate.FundingOfferId),
                out var funding);

            return new FundingExtensionStaging
            {
                OperationId = operationId,
                RolloverCandidateId = candidate.Id,
                SourceType = funding?.SourceType,
                SourceFundingRecordId = funding?.Id,
                RolloverStatus = candidate.RolloverStatus,
                ExclusionReason = candidate.ExclusionReason,
                NewFundingEndDate = candidate.NewFundingEndDate,
                FundingEndDate = funding?.FundingApprovalEndDate,
                FundingComments = funding?.Comments,
                CreatedAt = createdAt
            };
        }).ToList();
    }

    internal static void EnsureExpectedRowCount(
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


