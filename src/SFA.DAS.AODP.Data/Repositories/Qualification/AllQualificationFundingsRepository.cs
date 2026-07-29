using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

public class AllQualificationFundingsRepository : IAllQualificationFundingsRepository
{
    private readonly IApplicationDbContext _context;

    public AllQualificationFundingsRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AllQualificationFunding>> GetAsync(
        AllQualificationFundingFilter filter,
        CancellationToken cancellationToken)
    {
        var query = _context.AllQualificationFundings.AsNoTracking();

        if (filter.FundingOfferId.HasValue)
        {
            query = query.Where(f => f.FundingOfferId == filter.FundingOfferId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SourceType))
        {
            query = query.Where(f => f.SourceType == filter.SourceType);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
