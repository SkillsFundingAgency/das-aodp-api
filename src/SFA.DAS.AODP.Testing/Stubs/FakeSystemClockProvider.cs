using SFA.DAS.AODP.Data.Providers;

namespace SFA.DAS.AODP.Testing.Stubs;

public sealed class FakeSystemClockProvider(DateOnly today) : ISystemClockProvider
{
    public DateTime UtcNow => new(today.Year, today.Month, today.Day);

    public DateOnly Today { get; } = today;
}