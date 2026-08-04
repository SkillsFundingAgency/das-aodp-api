namespace SFA.DAS.AODP.Infrastructure.Extensions
{
    public static class NullableExtensions
    {
        public static string OrEmpty(this string? value)
            => value ?? string.Empty;

        public static bool OrFalse(this bool? value)
            => value ?? false;

        public static int ToIntOrDefault(this string? value, int defaultValue = 0)
            => int.TryParse(value, out var result)
                ? result
                : defaultValue;

        public static DateOnly? ToDateOnlyOrNull(this DateTime? value)
            => value.HasValue
                ? DateOnly.FromDateTime(value.Value)
                : null;
    }
}
