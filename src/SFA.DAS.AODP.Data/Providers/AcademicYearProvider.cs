using SFA.DAS.AODP.Data.Extensions;

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
}