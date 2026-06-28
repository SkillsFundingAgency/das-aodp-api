namespace SFA.DAS.AODP.Infrastructure.Services.Interfaces;

public interface ISystemClockService
{
    /// <summary>Retrieves the current system time in UTC.</summary>
    DateTime UtcNow { get; }

    DateOnly Today { get; }
}