using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Providers;

/// <summary>
/// Implementation of <see cref="ISystemClockProvider"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class SystemClockProvider : ISystemClockProvider
{
    /// <inheritdoc/>.
    public DateTime UtcNow => DateTime.UtcNow;
}