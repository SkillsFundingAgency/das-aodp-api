namespace SFA.DAS.AODP.Data.Repositories.Pldns;

/// <summary>
/// Provides an ability to query the PLDNS data set.
/// </summary>
public interface IPldnsRepository
{
    /// <summary>
    /// Get all the PLDNS records that currently exist.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all PLDNS records.</returns>
    Task<IList<Entities.Import.Pldns>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a PLDNS record by the Qan code, if it exists.
    /// </summary>
    /// <param name="qan">The Qan code to search upon.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the PLDNS record, or null if not found.</returns>
    Task<Entities.Import.Pldns?> GetPldnsByQanAsync(string qan, CancellationToken cancellationToken);
}