namespace SFA.DAS.AODP.Data.Repositories.Import;

public interface IImportRepository
{
    Task BulkInsertAsync<T>(IEnumerable<T> items, CancellationToken cancellationToken = default);

    Task<int> DeleteDuplicateAsync(string spName, string? qan = null, CancellationToken cancellationToken = default);
}