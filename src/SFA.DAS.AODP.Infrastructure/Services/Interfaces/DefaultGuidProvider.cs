using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Infrastructure.Services.Interfaces;

/// <summary>
/// Default implementation for <see cref="IGuidProvider"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class DefaultGuidProvider : IGuidProvider
{
    /// <inheritdoc/>.
    public Guid NewGuid() => Guid.NewGuid();
}