using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class DateTimeExtensions
{
    public static string ToCsvDateFormat(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy/MM/dd");
    }

    public static string ToCsvDateFormat(this DateOnly dateOnly)
    {
        return dateOnly.ToString("yyyy/MM/dd");
    }

    public static string ToDiscussionHistoryDateFormat(this DateOnly? dateOnly)
    {
        return dateOnly?.ToString("yyyy/MM/dd") ?? string.Empty;
    }

    public static string ToFilenameDateFormat(this DateOnly dateOnly)
    {
        return dateOnly.ToString("ddMMyyyy");
    }
}
