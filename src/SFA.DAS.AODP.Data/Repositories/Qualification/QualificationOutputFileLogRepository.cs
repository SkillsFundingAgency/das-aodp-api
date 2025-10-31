using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public class QualificationOutputFileLogRepository : IQualificationOutputFileLogRepository
    {
        private readonly IApplicationDbContext _context;

        public QualificationOutputFileLogRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateAsync(QualificationOutputFileLog entity, CancellationToken ct)
        {
            entity.Id = Guid.NewGuid();
            entity.Timestamp = DateTime.UtcNow;

            _context.QualificationOutputFileLog.Add(entity);
            await _context.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task<IReadOnlyList<QualificationOutputFileLog>> ListAsync(
            int? take = null,
            CancellationToken ct = default)
        {
            IQueryable<QualificationOutputFileLog> query = _context.QualificationOutputFileLog
                .AsNoTracking()
                .OrderByDescending(x => x.Timestamp);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync(ct);
        }

    }
}
