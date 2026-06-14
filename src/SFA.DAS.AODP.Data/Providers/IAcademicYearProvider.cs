namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Defines a means to calculate the end date of the current academic year based on the current date.
/// </summary>
public interface IAcademicYearProvider
{
    /// <summary>
    /// Gets the current academic year-end date, which is the 31st of July. If the current date is after the 31st of July, then this will return the 31st of July for the following year.
    /// </summary>
    /// <returns></returns>
    DateOnly GetCurrentAcademicYearEndDate();

    /// <summary>
    /// Determines whether the date is within the current academic year.
    /// </summary>
    /// <param name="dateToCheck">The date to check.</param>
    /// <returns><c>True</c> if the date falls within the current academic year, <c>False</c> otherwise.</returns>
    bool IsWithinCurrentAcademicYear(DateTime? dateToCheck);
}