using SFA.DAS.AODP.Data.Entities.Import;

namespace SFA.DAS.AODP.Data.Repositories.Import;

public interface IDefundingListRepository
{
    Task BulkInsertAsync(IEnumerable<DefundingList> items, CancellationToken cancellationToken = default);

    Task<int> DeleteDuplicateDefundingListsAsync(string? qan = null, CancellationToken cancellationToken = default);
}
