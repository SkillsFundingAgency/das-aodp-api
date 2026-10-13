namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Implementation for <see cref="IAcademicYearProvider"/>.
/// </summary>
/// <param name="clock">Abstraction over the system clock.</param>
public class AcademicYearProvider(ISystemClockProvider clock) : IAcademicYearProvider
{
    /// <inheritdoc/>.
    public DateOnly GetCurrentAcademicYearEndDate()
    {
        var today = clock.Today;

        if (today > new DateOnly(today.Year, 7, 31))
        {
            return new DateOnly(today.Year + 1, 7, 31);
        }

        return new DateOnly(today.Year, 7, 31);
    }

    /// <inheritdoc/>.
    public string GetCurrentAcademicYear()
    {
        var endYear = GetCurrentAcademicYearEndDate().Year;
        return $"{endYear - 1}/{endYear % 100:00}";
    }

    /// <inheritdoc/>.
    public DateOnly GetAcademicYearEndForDate(DateOnly dateOnly)
    {
        if (dateOnly > new DateOnly(dateOnly.Year, 7, 31))
        {
            return new DateOnly(dateOnly.Year + 1, 7, 31);
        }

        return new DateOnly(dateOnly.Year, 7, 31);
    }

    /// <inheritdoc/>.
    public bool IsWithinCurrentAcademicYear(DateTime? dateToCheck)
    {
        if (dateToCheck is null)
        {
            return false;
        }

        var currentAcademicYear = GetCurrentAcademicYearEndDate();
        if (DateOnly.FromDateTime(dateToCheck.Value) < currentAcademicYear && DateOnly.FromDateTime(dateToCheck.Value) >= new DateOnly(currentAcademicYear.Year - 1, 8, 1))
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc/>.
    public bool AreDatesWithinSameAcademicYear(DateTime? firstDateToCheck, DateOnly? secondDateToCheck)
    {
        if (firstDateToCheck is null || secondDateToCheck is null)
        {
            return false;
        }

        var academicYearForFirstDate = GetAcademicYearEndForDate(DateOnly.FromDateTime(firstDateToCheck.Value));
        var academicYearForSecondDate = GetAcademicYearEndForDate(secondDateToCheck.Value);

        return academicYearForFirstDate == academicYearForSecondDate;
    }
}
