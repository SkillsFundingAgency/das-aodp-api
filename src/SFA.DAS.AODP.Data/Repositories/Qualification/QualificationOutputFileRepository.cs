using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public class QualificationOutputFileRepository : IQualificationOutputFileRepository
    {
        private readonly IApplicationDbContext _context;

        public QualificationOutputFileRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QualificationOutputFile>> GetQualificationOutputFile()
        {
            return await _context.QualificationExport.ToListAsync<QualificationOutputFile>();
        }

        public async Task MarkPendingQaaQualificationsAsPublishedAsync(DateTime publishedAt, CancellationToken cancellationToken)
        {
            var pendingQaaQualifications = await _context.RegulatedQaaQualifications
                .Where(qualification =>
                    qualification.PublicationStatus == QaaPublicationStatus.PendingNew ||
                    qualification.PublicationStatus == QaaPublicationStatus.PendingChange)
                .ToListAsync(cancellationToken);

            foreach (var qualification in pendingQaaQualifications)
            {
                qualification.MarkAsPublished(publishedAt);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

