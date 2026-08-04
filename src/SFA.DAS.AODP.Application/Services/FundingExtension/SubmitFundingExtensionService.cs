using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Constants;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.FundingExtension;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Services.FundingExtension;

public class SubmitFundingExtensionService : ISubmitFundingExtensionService
{
    private static readonly Guid RolloverExtendedActionTypeId =
        Guid.Parse("00000000-0000-0000-0000-000000000004");

    private static readonly Guid RolloverNotExtendedActionTypeId =
        Guid.Parse("00000000-0000-0000-0000-000000000005");

    private readonly IFundingExtensionPersistenceRepository _persistenceRepository;
    private readonly ISystemClockService _clockService;
    private readonly IGuidProvider _guidProvider;
    private readonly ILogger<SubmitFundingExtensionService> _logger;

    public SubmitFundingExtensionService(
        IFundingExtensionPersistenceRepository persistenceRepository,
        ISystemClockService clockService,
        IGuidProvider guidProvider,
        ILogger<SubmitFundingExtensionService> logger)
    {
        _persistenceRepository = persistenceRepository;
        _clockService = clockService;
        _guidProvider = guidProvider;
        _logger = logger;
    }

    public async Task<bool> Submit(
        List<RolloverCandidates> candidates,
        List<FundingExtensionItem> inputItems,
        List<QualificationFundings> fundings,
        CancellationToken cancellationToken)
    {
        var totalStarted = Stopwatch.GetTimestamp();

        try
        {
            var lookupStarted = Stopwatch.GetTimestamp();
            var inputLookup = inputItems.ToDictionary(
                x => (x.Qan!, x.FundingStreamName!));

            var fundingLookup = fundings.ToDictionary(
                x => (x.QualificationVersionId, x.FundingOfferId));
            _logger.LogInformation(
                "Built funding-extension input and funding lookups for {InputCount} inputs and {FundingCount} fundings in {ElapsedMilliseconds} ms",
                inputItems.Count,
                fundings.Count,
                Stopwatch.GetElapsedTime(lookupStarted).TotalMilliseconds);

            var updateStarted = Stopwatch.GetTimestamp();
            var (updatedCandidates, updatedFundings) = ApplyCandidateAndFundingUpdates(
                candidates,
                inputLookup,
                fundingLookup);
            _logger.LogInformation(
                "Prepared {CandidateUpdateCount} candidate updates and {FundingUpdateCount} funding updates in {ElapsedMilliseconds} ms",
                updatedCandidates.Count,
                updatedFundings.Count,
                Stopwatch.GetElapsedTime(updateStarted).TotalMilliseconds);

            var historyStarted = Stopwatch.GetTimestamp();
            var historyEntries = CreateDiscussionHistories(updatedCandidates, fundingLookup);
            _logger.LogInformation(
                "Created {HistoryCount} rollover discussion-history records in {ElapsedMilliseconds} ms",
                historyEntries.Count,
                Stopwatch.GetElapsedTime(historyStarted).TotalMilliseconds);

            var persistenceStarted = Stopwatch.GetTimestamp();
            await _persistenceRepository.PersistAsync(
                updatedCandidates,
                updatedFundings,
                historyEntries,
                cancellationToken);
            _logger.LogInformation(
                "Persisted funding-extension changes in {ElapsedMilliseconds} ms",
                Stopwatch.GetElapsedTime(persistenceStarted).TotalMilliseconds);

            _logger.LogInformation(
                "Completed funding-extension processing in {ElapsedMilliseconds} ms",
                Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds);

            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Funding-extension processing failed after {ElapsedMilliseconds} ms for {CandidateCount} candidates",
                Stopwatch.GetElapsedTime(totalStarted).TotalMilliseconds,
                candidates.Count);
            return false;
        }
    }

    private static (List<RolloverCandidates> Candidates, List<QualificationFundings> Fundings)
        ApplyCandidateAndFundingUpdates(
            List<RolloverCandidates> candidates,
            IReadOnlyDictionary<(string Qan, string FundingStreamName), FundingExtensionItem> inputLookup,
            IReadOnlyDictionary<(Guid QualificationVersionId, Guid FundingOfferId), QualificationFundings> fundingLookup)
    {
        var updatedCandidates = new List<RolloverCandidates>();
        var updatedFundings = new List<QualificationFundings>();

        foreach (var candidate in candidates)
        {
            if (!inputLookup.TryGetValue(
                    (candidate.QualificationVersion.Qualification.Qan, candidate.FundingOffer.Name),
                    out var input))
            {
                continue;
            }

            var status = RolloverStatusInfo.FromCsv(input.RolloverStatus ?? string.Empty);

            switch (status)
            {
                case RolloverStatus.Extended:
                    candidate.SetExtended(input.ProposedFundingApprovalEndDate);

                    if (fundingLookup.TryGetValue(
                            (candidate.QualificationVersionId, candidate.FundingOfferId),
                            out var funding))
                    {
                        funding.EndDate = DateOnly.FromDateTime(input.ProposedFundingApprovalEndDate);
                        funding.Comments = input.Comments;
                        updatedFundings.Add(funding);
                    }

                    break;

                case RolloverStatus.Excluded:
                    candidate.SetExcluded(input.ExclusionReason!);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unexpected rollover status: {input.RolloverStatus}");
            }

            updatedCandidates.Add(candidate);
        }

        return (updatedCandidates, updatedFundings);
    }

    private List<QualificationDiscussionHistory> CreateDiscussionHistories(
        IReadOnlyCollection<RolloverCandidates> candidates,
        IReadOnlyDictionary<(Guid QualificationVersionId, Guid FundingOfferId), QualificationFundings> fundingLookup)
    {
        var historyEntries = new List<QualificationDiscussionHistory>();

        foreach (var group in candidates.GroupBy(c => c.QualificationVersion.QualificationId))
        {
            var extended = group
                .Where(c => c.RolloverStatus == RolloverStatus.Extended)
                .ToList();

            var excluded = group
                .Where(c => c.RolloverStatus == RolloverStatus.Excluded)
                .ToList();

            if (extended.Count > 0)
            {
                var lines = extended.Select(candidate =>
                {
                    fundingLookup.TryGetValue(
                        (candidate.QualificationVersionId, candidate.FundingOfferId),
                        out var funding);

                    var endDate = funding?.EndDate.ToFundingEndDateFormat();
                    return $"{candidate.FundingOffer.Name} extended to {endDate}";
                });

                historyEntries.Add(CreateDiscussionHistoryEntry(
                    string.Join("\n", lines),
                    RolloverExtendedActionTypeId,
                    group.Key));
            }

            if (excluded.Count > 0)
            {
                var lines = excluded.Select(candidate =>
                    $"{candidate.FundingOffer.Name} was not extended due to {candidate.ExclusionReason}");

                historyEntries.Add(CreateDiscussionHistoryEntry(
                    string.Join("\n", lines),
                    RolloverNotExtendedActionTypeId,
                    group.Key));
            }
        }

        return historyEntries;
    }

    private QualificationDiscussionHistory CreateDiscussionHistoryEntry(
        string note,
        Guid actionTypeId,
        Guid qualificationId)
    {
        return new QualificationDiscussionHistory
        {
            Id = _guidProvider.NewGuid(),
            Title = "Rollover Funding Decision",
            UserDisplayName = "Rollover System",
            ActionTypeId = actionTypeId,
            QualificationId = qualificationId,
            Notes = note,
            Timestamp = _clockService.UtcNow
        };
    }
}
