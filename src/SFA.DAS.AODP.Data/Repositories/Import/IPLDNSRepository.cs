using SFA.DAS.AODP.Data.Entities.Import;

namespace SFA.DAS.AODP.Data.Repositories.Import;

public interface IPLDNSRepository
{
    Task BulkInsertAsync(IEnumerable<PLDNS> items, CancellationToken cancellationToken = default);
    Task<int> DeleteDuplicatePLDNSAsync(string? qan = null, CancellationToken cancellationToken = default);
}
