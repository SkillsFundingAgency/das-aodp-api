using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.QaaQualification;

public class QaaQualificationFundingsRepository : IQaaQualificationFundingsRepository
{
    private readonly IApplicationDbContext _context;

    public QaaQualificationFundingsRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(QaaQualificationFunding funding, CancellationToken cancellationToken)
    {
        _context.QaaQualificationFundings.Add(funding);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<QaaQualificationFunding>> GetByQaaQualificationIdAsync(
        Guid qaaQualificationId,
        CancellationToken cancellationToken)
    {
        return await _context.QaaQualificationFundings
            .Include(q => q.QaaQualification)
            .Include(q => q.FundingOffer)
            .Where(q => q.QaaQualificationId == qaaQualificationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<QaaQualificationFunding>> GetRolloverQaaQualificationFundingsAsync(
        List<SourceQualificationFundingKey> candidates,
        CancellationToken cancellationToken)
    {
        var sourceQualificationIds = candidates
            .Select(x => x.SourceQualificationId)
            .Distinct()
            .ToList();
        var fundingOfferIds = candidates
            .Select(x => x.FundingOfferId)
            .Distinct()
            .ToList();
        var keys = candidates
            .Select(x => (x.SourceQualificationId, x.FundingOfferId))
            .ToHashSet();

        var fundings = await _context.QaaQualificationFundings
            .Where(qf =>
                sourceQualificationIds.Contains(qf.QaaQualificationId) &&
                fundingOfferIds.Contains(qf.FundingOfferId))
            .ToListAsync(cancellationToken);

        return fundings
            .Where(qf => keys.Contains((qf.QaaQualificationId, qf.FundingOfferId)))
            .ToList();
    }
}
