namespace SFA.DAS.AODP.Application.Constants
{
    public static class RolloverStatuses
    {
        public const string ToExtend = "To Extend";
        public const string ToExclude = "To Exclude";

        public static readonly IReadOnlyList<string> All = 
        [
            ToExtend,
            ToExclude
        ];

        public static string ToList()
        {
            if (All.Count == 1)
                return All[0];

            if (All.Count == 2)
                return $"{All[0]} or {All[1]}";

            return string.Join(", ", All.Take(All.Count - 1))
                 + " or "
                 + All.Last();
        }
    }
}
