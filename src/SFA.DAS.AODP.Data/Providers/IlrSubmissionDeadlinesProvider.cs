using SFA.DAS.AODP.Data.Extensions;

namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Default implementation for <see cref="IIlrSubmissionDeadlinesProvider"/>.
/// </summary>
public class IlrSubmissionDeadlinesProvider(ISystemClockProvider clock) : IIlrSubmissionDeadlinesProvider
{
    /// <inheritdoc/>.
    public IlrSubmissionDeadline GetFinalSubmissionDeadline()
    {
        // The R14 deadline is calculated as follows:
        // We first need the R02 deadline for the NEXT year.
        // So, presume we are in 24/25 academic year based on current date, we want to find the R14 deadline, we need the R02 deadline for NEXT year first.
        // So, the R02 deadline is always the 4th working day of the month, so we first resolve that.
        // Then we add 14 days (2 weeks) to that date and find the closest Thursday, this then becomes the R14 deadline.

        // Note: The R14 deadline is always in the NEXT academic year date wise, so if we are in 2024/25 academic year, the R14 deadline will be in 2025/26 academic year, but it operates as a late submission deadline for the previous academic year.

        var today = clock.Today;
        var startingDate = new DateTime(today.Year, 10, 1);
        var r02DeadlineDateForNextYear = startingDate.GetSpecificWorkingDateOfMonth(startingDate.Year, startingDate.Month, 4);

        return new IlrSubmissionDeadline("R14", DateOnly.FromDateTime(r02DeadlineDateForNextYear.AddDays(14).GetClosestDayOfWeek(DayOfWeek.Thursday)));
    }
}