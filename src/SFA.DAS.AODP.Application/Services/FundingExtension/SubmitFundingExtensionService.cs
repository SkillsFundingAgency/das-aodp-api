using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Constants;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
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
        List<RolloverFundingUpdate> fundingUpdates,
        CancellationToken cancellationToken)
    {
        var totalStarted = Stopwatch.GetTimestamp();

        try
        {
            var updateStarted = Stopwatch.GetTimestamp();
            var updatedCandidates = ApplyCandidateAndFundingUpdates(
                candidates,
                inputItems,
                fundingUpdates,
                _clockService.UtcNow);
            _logger.LogInformation(
                "Prepared {CandidateUpdateCount} candidate updates against {FundingUpdateCount} funding updates in {ElapsedMilliseconds} ms",
                updatedCandidates.Count,
                fundingUpdates.Count,
                Stopwatch.GetElapsedTime(updateStarted).TotalMilliseconds);

            var historyStarted = Stopwatch.GetTimestamp();
            var historyEntries = CreateDiscussionHistories(updatedCandidates, fundingUpdates);
            var qaaHistoryEntries = CreateQaaDiscussionHistories(updatedCandidates, fundingUpdates);
            _logger.LogInformation(
                "Created {HistoryCount} rollover discussion-history records ({QaaHistoryCount} QAA) in {ElapsedMilliseconds} ms",
                historyEntries.Count,
                qaaHistoryEntries.Count,
                Stopwatch.GetElapsedTime(historyStarted).TotalMilliseconds);

            var persistenceStarted = Stopwatch.GetTimestamp();
            await _persistenceRepository.PersistAsync(
                updatedCandidates,
                fundingUpdates,
                historyEntries,
                qaaHistoryEntries,
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

    private static List<RolloverCandidates> ApplyCandidateAndFundingUpdates(
        List<RolloverCandidates> candidates,
        List<FundingExtensionItem> inputItems,
        List<RolloverFundingUpdate> fundingUpdates,
        DateTime updatedAt)
    {
        var inputLookup = inputItems.ToDictionary(
            x => CandidateKey.Create(x.Qan, x.FundingStreamName));

        var fundingLookup = fundingUpdates.ToDictionary(
            x => (x.SourceType, x.SourceQualificationId, x.FundingOfferId, x.AcademicYear));

        var updatedCandidates = new List<RolloverCandidates>();

        foreach (var c in candidates)
        {
            if (!inputLookup.TryGetValue(
                CandidateKey.Create(c.SourceQualificationReference, c.FundingOffer.Name),
                out var input))
            {
                continue;
            }

            var status = RolloverStatusInfo.FromCsv(input.RolloverStatus ?? string.Empty);

            switch (status)
            {
                case RolloverStatus.Extended:
                    c.SetExtended(input.ProposedFundingApprovalEndDate);

                    if (fundingLookup.TryGetValue(
                        (c.SourceType, c.SourceQualificationId, c.FundingOfferId, c.AcademicYear),
                        out var funding))
                    {
                        funding.ApplyFundingEndDate(
                            DateOnly.FromDateTime(input.ProposedFundingApprovalEndDate),
                            input.Comments,
                            updatedAt);
                    }
                    break;

                case RolloverStatus.Excluded:
                    c.SetExcluded(input.ExclusionReason!);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unexpected rollover status: {input.RolloverStatus}");
            }

            updatedCandidates.Add(c);
        }

        return updatedCandidates;
    }

    private List<QualificationDiscussionHistory> CreateDiscussionHistories(
        IReadOnlyCollection<RolloverCandidates> candidates,
        IReadOnlyCollection<RolloverFundingUpdate> fundingUpdates)
    {
        var historyEntries = new List<QualificationDiscussionHistory>();

        var groups = candidates
            .Where(c => c.DiscussionQualificationId.HasValue)
            .GroupBy(c => c.DiscussionQualificationId!.Value);

        foreach (var group in groups)
        {
            var qualificationId = group.Key;

            var extended = group
                .Where(c => c.RolloverStatus == RolloverStatus.Extended)
                .ToList();

            var excluded = group
                .Where(c => c.RolloverStatus == RolloverStatus.Excluded)
                .ToList();

            if (extended.Count > 0)
            {
                var lines = extended.Select(c =>
                {
                    var f = fundingUpdates.FirstOrDefault(x =>
                        x.SourceType == c.SourceType &&
                        x.SourceQualificationId == c.SourceQualificationId &&
                        x.FundingOfferId == c.FundingOfferId &&
                        x.AcademicYear == c.AcademicYear);

                    var endDate = f?.FundingApprovalEndDate.ToFundingEndDateFormat();

                    return $"{c.FundingOffer.Name} extended to {endDate}";
                });

                historyEntries.Add(CreateDiscussionHistoryEntry(
                    string.Join("\n", lines),
                    RolloverExtendedActionTypeId,
                    qualificationId));
            }

            if (excluded.Count > 0)
            {
                var lines = excluded.Select(c =>
                    $"{c.FundingOffer.Name} was not extended due to {c.ExclusionReason}");

                historyEntries.Add(CreateDiscussionHistoryEntry(
                    string.Join("\n", lines),
                    RolloverNotExtendedActionTypeId,
                    qualificationId));
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

    private List<QaaQualificationDiscussionHistory> CreateQaaDiscussionHistories(
        IReadOnlyCollection<RolloverCandidates> candidates,
        IReadOnlyCollection<RolloverFundingUpdate> fundingUpdates)
    {
        var historyEntries = new List<QaaQualificationDiscussionHistory>();

        var groups = candidates
            .Where(c => c.SourceType == RolloverSourceTypes.Qaa)
            .GroupBy(c => c.SourceQualificationId);

        foreach (var group in groups)
        {
            var qaaQualificationId = group.Key;

            var extended = group
                .Where(c => c.RolloverStatus == RolloverStatus.Extended)
                .ToList();

            var excluded = group
                .Where(c => c.RolloverStatus == RolloverStatus.Excluded)
                .ToList();

            if (extended.Count > 0)
            {
                var lines = extended.Select(c =>
                {
                    var f = fundingUpdates.FirstOrDefault(x =>
                        x.SourceType == c.SourceType &&
                        x.SourceQualificationId == c.SourceQualificationId &&
                        x.FundingOfferId == c.FundingOfferId &&
                        x.AcademicYear == c.AcademicYear);

                    var endDate = f?.FundingApprovalEndDate.ToFundingEndDateFormat();

                    return $"{c.FundingOffer.Name} extended to {endDate}";
                });

                historyEntries.Add(CreateQaaDiscussionHistoryEntry(
                    string.Join("\n", lines),
                    RolloverExtendedActionTypeId,
                    qaaQualificationId));
            }

            if (excluded.Count > 0)
            {
                var lines = excluded.Select(c =>
                    $"{c.FundingOffer.Name} was not extended due to {c.ExclusionReason}");

                historyEntries.Add(CreateQaaDiscussionHistoryEntry(
                    string.Join("\n", lines),
                    RolloverNotExtendedActionTypeId,
                    qaaQualificationId));
            }
        }

        return historyEntries;
    }

    private QaaQualificationDiscussionHistory CreateQaaDiscussionHistoryEntry(
        string note,
        Guid actionTypeId,
        Guid qaaQualificationId)
    {
        return new QaaQualificationDiscussionHistory
        {
            Id = _guidProvider.NewGuid(),
            Title = "Rollover Funding Decision",
            UserDisplayName = "Rollover System",
            ActionTypeId = actionTypeId,
            QaaQualificationId = qaaQualificationId,
            Notes = note,
            Timestamp = _clockService.UtcNow
        };
    }
}
