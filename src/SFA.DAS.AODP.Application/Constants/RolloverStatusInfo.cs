using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Constants
{
    public static class RolloverStatusInfo
    {
        public static RolloverStatus FromCsv(string csvValue)
        {
            if (csvValue is null)
                return RolloverStatus.Unknown;

            return csvValue.Trim().ToLowerInvariant() switch
            {
                "to extend" => RolloverStatus.Extended,
                "to exclude" => RolloverStatus.Rejected,
                "needs review" => RolloverStatus.NeedsReview,
                _ => RolloverStatus.Unknown
            };
        }


        public static string ToDisplay(this RolloverStatus status) =>
            status switch
            {
                RolloverStatus.Extended => "Extended",
                RolloverStatus.Rejected => "Excluded",
                RolloverStatus.NeedsReview => "Needs Review",
                _ => status.ToString()
            };
    }

}
