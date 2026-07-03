namespace SFA.DAS.AODP.Application.Extensions
{
    public static class QanFormatter
    {
        public static string RemoveSlashes(this string input)
        {
            input ??= string.Empty;
            return input.Replace("/", "");
        }
    }
}
