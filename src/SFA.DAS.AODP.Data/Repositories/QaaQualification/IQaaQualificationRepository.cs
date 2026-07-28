using SFA.DAS.AODP.Data.Entities.QaaQualification;

namespace SFA.DAS.AODP.Data.Repositories.QaaQualification;

/// <summary>
/// Defines a repository for managing <see cref="RegulatedQaaQualification"/>.
/// </summary>
public interface IQaaQualificationRepository
{
    /// <summary>
    /// Retrieves all entries of <see cref="RegulatedQaaQualification"/>.
    /// </summary>
    /// <param name="cancellationToken">Propagates a notification to cancel the operation.</param>
    /// <returns>Collection of <see cref="RegulatedQaaQualification"/>.</returns>
    Task<IEnumerable<RegulatedQaaQualification>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Save changes.
    /// </summary>
    /// <param name="cancellationToken">Propagates a notification to cancel the operation.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}