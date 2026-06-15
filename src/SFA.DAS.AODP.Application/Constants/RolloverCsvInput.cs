namespace SFA.DAS.AODP.Application.Constants
{
    public static class RolloverCsvInput
    {
        public const string ToExtend = "To Extend";
        public const string ToExclude = "To Exclude";

        public static readonly IReadOnlyList<string> AllowedStatuses =
        [
            ToExtend,
            ToExclude
        ];
    }

}
