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
        var today = clock.Today;
        var startingDate = new DateTime(today.Year, 10, 1);
        var r02DeadlineDate = startingDate.GetSpecificWorkingDateOfMonth(startingDate.Year, startingDate.Month, 3);

        return new IlrSubmissionDeadline("R14", DateOnly.FromDateTime(r02DeadlineDate.AddDays(14).GetClosestDayOfWeek(DayOfWeek.Thursday)));
    }
}