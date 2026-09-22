using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;

namespace SFA.DAS.AODP.Data.Repositories.QaaQualification;

public class QaaQualificationDownloadLogRepository : IQaaQualificationDownloadLogRepository
{
    private readonly IApplicationDbContext _context;

    public QaaQualificationDownloadLogRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(QaaQualificationDownloadLog entity, CancellationToken ct)
    {
        entity.Id = Guid.NewGuid();
        _context.QaaQualificationDownloadLog.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<IReadOnlyList<QaaQualificationDownloadLog>> ListAsync(
        int? take = null,
        CancellationToken ct = default)
    {
        IQueryable<QaaQualificationDownloadLog> query = _context.QaaQualificationDownloadLog
            .AsNoTracking()
            .OrderByDescending(x => x.DownloadDate);

        if (take.HasValue)
            query = query.Take(take.Value);

        return await query.ToListAsync(ct);
    }
}
