using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Constants;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Services.FundingExtension
{
    public class SubmitFundingExtensionService :ISubmitFundingExtensionService
    {
        private readonly IRolloverRepository _rolloverRepository;
        private readonly IQualificationDiscussionHistoryRepository _qualificationDiscussionHistoryRepository;
        private readonly ISystemClockService _clockService;
        private readonly IGuidProvider _guidProvider;
        private Guid RolloverExtendedActionTypeId = Guid.Parse("00000000-0000-0000-0000-000000000004");
        private Guid RolloverNotExtendedActionTypeId = Guid.Parse("00000000-0000-0000-0000-000000000005");

        public SubmitFundingExtensionService(IRolloverRepository rolloverRepository, IQualificationDiscussionHistoryRepository qualificationDiscussionHistoryRepository, ISystemClockService clockService, IGuidProvider guidProvider)
        {
            _rolloverRepository = rolloverRepository;
            _qualificationDiscussionHistoryRepository = qualificationDiscussionHistoryRepository;
            _clockService = clockService;
            _guidProvider = guidProvider;
        }

        public async Task<bool> Submit(
            List<RolloverCandidates> candidates,
            List<FundingExtensionItem> inputItems,
            List<QualificationFundings> fundings,
            CancellationToken cancellationToken)
        {
            try
            {
                ApplyCandidateAndFundingUpdates(candidates, inputItems, fundings);

                await CreateDiscussionHistoriesAsync(candidates, fundings);

                await _rolloverRepository.DeleteAllWorkflowCandidatesAsync(cancellationToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ApplyCandidateAndFundingUpdates(
            List<RolloverCandidates> candidates,
            List<FundingExtensionItem> inputItems,
            List<QualificationFundings> fundings)
        {
            var inputLookup = inputItems.ToDictionary(
                x => (x.Qan!, x.FundingStreamName!));

            var fundingLookup = fundings.ToDictionary(
                x => (x.QualificationVersionId, x.FundingOfferId));

            foreach (var c in candidates)
            {
                if (!inputLookup.TryGetValue(
                    (c.QualificationVersion.Qualification.Qan, c.FundingOffer.Name),
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
                            (c.QualificationVersionId, c.FundingOfferId),
                            out var funding))
                        {
                            funding.EndDate = DateOnly.FromDateTime(input.ProposedFundingApprovalEndDate);
                            funding.Comments = input.Comments;
                        }
                        break;

                    case RolloverStatus.Excluded:
                        c.SetExcluded(input.ExclusionReason!);
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unexpected rollover status: {input.RolloverStatus}");
                }
            }
        }

        private async Task CreateDiscussionHistoriesAsync(
            List<RolloverCandidates> candidates,
            List<QualificationFundings> fundings)
        {
            var historyEntries = new List<QualificationDiscussionHistory>();

            var groups = candidates
                .GroupBy(c => c.QualificationVersion.QualificationId);

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
                        var f = fundings.FirstOrDefault(x =>
                            x.QualificationVersionId == c.QualificationVersionId &&
                            x.FundingOfferId == c.FundingOfferId);

                        var endDate = f?.EndDate.ToDiscussionHistoryDateFormat();

                        return $"{c.FundingOffer.Name} extended to {endDate}";
                    });

                    var notes = string.Join("\n", lines);

                    historyEntries.Add(CreateDiscussionHistoryEntry(notes, RolloverExtendedActionTypeId, qualificationId));
                }

                if (excluded.Count > 0)
                {
                    var lines = excluded.Select(c =>
                        $"{c.FundingOffer.Name} was not extended due to {c.ExclusionReason}");

                    var notes = string.Join("\n", lines);

                    historyEntries.Add(CreateDiscussionHistoryEntry(notes, RolloverNotExtendedActionTypeId, qualificationId));
                }
            }

            _qualificationDiscussionHistoryRepository.AddDiscussionHistories(historyEntries);
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
}