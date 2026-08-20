using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public class QualificationFundingsRepository : IQualificationFundingsRepository
    {
        private readonly IApplicationDbContext _context;

        public QualificationFundingsRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<QualificationFundings>> GetByIdAsync(Guid qualificationVersionId)
        {
            return await _context
                        .QualificationFundings
                        .Include(a => a.QualificationVersion)
                        .ThenInclude(b => b.Qualification)
                        .Include(a => a.FundingOffer)
                        .Where(v => v.QualificationVersionId == qualificationVersionId)
                        .ToListAsync();

        }

        public async Task UpdateAsync(List<QualificationFundings> qualificationFundings)
        {
            _context.QualificationFundings.UpdateRange(qualificationFundings);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(List<QualificationFundings> qualificationFundings)
        {
            foreach (var qualificationFunding in qualificationFundings)
            {
                qualificationFunding.Id = Guid.NewGuid();
                _context.QualificationFundings.Add(qualificationFunding);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<QualificationFundings>> GetRolloverQualificationFundingsAsync(
    List<QualificationFundingKey> candidates,
    CancellationToken cancellationToken)
        {
            if (candidates.Count == 0)
            {
                return [];
            }

            var sourceQualificationIds = candidates
                .Select(x => x.SourceQualificationId)
                .Distinct()
                .ToList();

            var fundingOfferIds = candidates
                .Select(x => x.FundingOfferId)
                .Distinct()
                .ToList();

            var fundings = await _context.QualificationFundings
                .AsNoTracking()
                .Where(qf =>
                    sourceQualificationIds.Contains(qf.QualificationVersionId) &&
                    fundingOfferIds.Contains(qf.FundingOfferId))
                .ToListAsync(cancellationToken);

            var keySet = candidates
                .Select(x => (x.SourceQualificationId, x.FundingOfferId))
                .ToHashSet();

            return fundings
                .Where(x => keySet.Contains((x.QualificationVersionId, x.FundingOfferId)))
                .ToList();
        }
    }
}
