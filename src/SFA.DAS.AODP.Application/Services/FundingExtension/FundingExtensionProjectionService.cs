using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Constants;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Services.FundingExtension
{
    public class FundingExtensionProjectionService : IFundingExtensionProjectionService
    {
        public FundingExtensionSummary ProjectSummary(
            List<RolloverCandidateStatusItem> dbCandidates,
            List<RolloverCandidateForValidation> uploadedCandidates)
        {
            var uploadedDistinct = uploadedCandidates
                .GroupBy(x => new { x.Qan, x.FundingStreamName })
                .Select(g => g.First())
                .ToList();

            var uploadedLookup = uploadedDistinct.ToDictionary(
                x => (x.Qan, x.FundingStreamName),
                x => x.RollOverStatus);

            var projectedStatuses = dbCandidates
                .Select(db =>
                {
                    var key = (db.Qan, db.FundingStreamName);

                    if (uploadedLookup.TryGetValue(key, out var csvStatus))
                    {
                        return RolloverStatusInfo.FromCsv(csvStatus);
                    }

                    return db.RolloverStatus;
                })
                .ToList();

            var extendedInUploadCount =
                uploadedCandidates
                    .Select(x => RolloverStatusInfo.FromCsv(x.RollOverStatus))
                    .Count(s => s == RolloverStatus.Extended);
            var extendedCount = projectedStatuses.Count(x => x == RolloverStatus.Extended);
            var excludedCount = projectedStatuses.Count(x => x == RolloverStatus.Rejected);
            var pendingCount = projectedStatuses.Count(x => x == RolloverStatus.NeedsReview);

            return new FundingExtensionSummary
            {
                TotalCandidatesCount = dbCandidates.Count,
                CandidatesExtendedInUploadCount = extendedInUploadCount,
                TotalCandidatesToBeExtendedCount = extendedCount,
                TotalCandidatesToBeExcludedCount = excludedCount,
                TotalCandidatesToBeReviewedCount = pendingCount
            };
        }
    }

}
