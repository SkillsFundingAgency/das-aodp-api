namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Provides an abstraction for accessing the system clock, allowing for easier testing and time manipulation.
/// </summary>
public interface ISystemClockProvider
{
    /// <summary>
    /// The current system time in Coordinated Universal Time (UTC).
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Today's date in <see cref="DateOnly"/> format, derived from the current UTC time.
    /// </summary>
    /// <remarks>
    /// It might not seem like it initially, but using the UtcNow property is important, this is to ensure that any date calculations are consistent and not affected by the local time zone of the server or environment where the code is running. By using UtcNow, we can avoid issues related to daylight saving time changes and ensure that our date calculations are based on a standard reference point (UTC).
    /// </remarks>
    public DateOnly Today => DateOnly.FromDateTime(UtcNow);
}