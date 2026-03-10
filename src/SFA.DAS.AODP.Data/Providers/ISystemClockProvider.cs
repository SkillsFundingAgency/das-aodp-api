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
}