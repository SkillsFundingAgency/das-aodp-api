using SFA.DAS.AODP.Data.Entities.Import;

namespace SFA.DAS.AODP.Data.Repositories.Import;

public interface IImportRepository
{
    Task BulkInsertAsync(List<DefundingList> items, CancellationToken cancellationToken = default);

    Task<int> DeleteDuplicateAsync(string spName, string? qan = null, CancellationToken cancellationToken = default);
}