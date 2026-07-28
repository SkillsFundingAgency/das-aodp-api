using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Repositories.Pldns;

/// <summary>
/// Default implementation for <see cref="IPldnsRepository"/>.
/// </summary>
/// <param name="dbContext">Provides access to the underlying <see cref="DbContext"/> sets.</param>
public class PldnsRepository(IApplicationDbContext dbContext) : IPldnsRepository
{
    private readonly IApplicationDbContext _dbContext = dbContext;

    /// <inheritdoc/>.
    public async Task<IList<Entities.Import.Pldns>> GetAllAsync(CancellationToken cancellationToken) 
        => await _dbContext.Pldns.ToListAsync(cancellationToken);

    /// <inheritdoc/>.
    public async Task<Entities.Import.Pldns?> GetPldnsByQanAsync(string qan, CancellationToken cancellationToken) 
        => await _dbContext.Pldns.FirstOrDefaultAsync(p => p.Qan == qan, cancellationToken);
}