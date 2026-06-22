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

        public async Task RemoveAsync(List<QualificationFundings> qualificationFundings)
        {
            _context.QualificationFundings.RemoveRange(qualificationFundings);
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
            var keySet = candidates
                .Select(x => x.QualificationVersionId + "|" +  x.FundingOfferId )
                .ToList();

            return await _context.QualificationFundings
                .AsNoTracking()
                .Where(qf => keySet.Contains(qf.QualificationVersionId + "|" + qf.FundingOfferId))
                .ToListAsync(cancellationToken);
        }
    }
}
