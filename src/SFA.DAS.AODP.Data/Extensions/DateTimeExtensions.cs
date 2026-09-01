namespace SFA.DAS.AODP.Data.Extensions;

/// <summary>
/// Extension methods for <see cref="DateTime"/> to support data calculations and formatting.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Get the working day of the month for a given year, month and working day number. For example, if you want to get the date for the 3rd working day of October 2024, you would call GetSpecificWorkingDateOfMonth(2024, 10, 3) and it would return the date for the 3rd working day in October 2024, skipping any weekends. This is useful for calculating deadlines or important dates that fall on specific working days of a month.
    /// </summary>
    /// <param name="dateTime">Initial datetime.</param>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="workingDay">The working day number.</param>
    /// <returns>The specific working date.</returns>
    public static DateTime GetSpecificWorkingDateOfMonth(this DateTime dateTime, int year, int month, int workingDay)
    {
        var startingDate = new DateOnly(year, month, 1);
        return Enumerable.Range(1, DateTime.DaysInMonth(startingDate.Year, month))
            .Select(day => new DateTime(startingDate.Year, month, day))
            .Where(date => date.DayOfWeek is not DayOfWeek.Saturday && date.DayOfWeek is not DayOfWeek.Sunday)
            .ElementAt(workingDay - 1);
    }

    public static DateTime GetClosestDayOfWeek(this DateTime date, DayOfWeek dayOfWeek)
    {
        var currentDayNum = (int)date.DayOfWeek;
        var targetDayNum = (int)dayOfWeek;

        // Calculate initial raw difference (-6 to 6 days)
        var daysUntilTarget = targetDayNum - currentDayNum;

        // If the target is more than 3 days ahead, the closest one is in the past week
        if (daysUntilTarget > 3)
        {
            daysUntilTarget -= 7;
        }
        // If the target is more than 3 days behind, the closest one is in the next week
        else if (daysUntilTarget < -3)
        {
            daysUntilTarget += 7;
        }

        return date.AddDays(daysUntilTarget);
    }
}