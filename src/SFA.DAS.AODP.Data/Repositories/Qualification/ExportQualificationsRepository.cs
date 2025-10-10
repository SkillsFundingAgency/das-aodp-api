using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public class ExportQualificationsRepository : IExportQualificationsRepository
    {
        private readonly IApplicationDbContext _context;

        public ExportQualificationsRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QualificationExport>> GetQualificationExport()
        {
            return await _context.QualificationExport.ToListAsync<QualificationExport>();
        }
    }
}


