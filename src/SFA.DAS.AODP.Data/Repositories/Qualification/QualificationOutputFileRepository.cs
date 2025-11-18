using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
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
    }
}


