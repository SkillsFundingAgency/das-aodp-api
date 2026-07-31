using SFA.DAS.AODP.Infrastructure.Services.Interfaces;

namespace SFA.DAS.AODP.Infrastructure.Services;

public class SystemClockService : ISystemClockService
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}