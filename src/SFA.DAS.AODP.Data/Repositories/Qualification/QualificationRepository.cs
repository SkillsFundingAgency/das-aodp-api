using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public class QualificationRepository : IQualificationRepository
    {
        private readonly IApplicationDbContext _context;

        public QualificationRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Entities.Qualification.Qualification> GetByIdAsync(string qualificationReference)
        {
            return await _context
                        .Qualification
                        .Include(a => a.QualificationVersions)
                        .Where(v => v.Qan == qualificationReference)
                        .FirstOrDefaultAsync();

        }
    }
}
