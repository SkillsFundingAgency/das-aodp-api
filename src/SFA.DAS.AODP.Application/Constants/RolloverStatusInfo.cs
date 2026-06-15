using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Constants
{
    public static class RolloverStatusInfo
    {
        public static RolloverStatus FromCsv(string csvValue) =>
            csvValue switch
            {
                RolloverCsvInput.ToExtend => RolloverStatus.Extended,
                RolloverCsvInput.ToExclude => RolloverStatus.Rejected,
                _ => throw new ArgumentException($"Unknown CSV status: {csvValue}")
            };

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
