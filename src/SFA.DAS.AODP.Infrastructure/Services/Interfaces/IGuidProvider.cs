namespace SFA.DAS.AODP.Infrastructure.Services.Interfaces;

/// <summary>
/// Defines a simple wrapper abstraction over the <see cref="Guid.NewGuid"/> for better testability.
/// </summary>
public interface IGuidProvider
{
    /// <summary>
    /// Generates a new guid.
    /// </summary>
    /// <returns>A Guid.</returns>
    Guid NewGuid();
}