using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using Qualification = Entities.Qualification.Qualification;

public class QualificationsRepository(ApplicationDbContext context) : IQualificationsRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<Qualification>> GetChangedQualificationsAsync()
    {
        return await _context.Qualifications.Where(q => q.QualificationVersions.Any())
            .ToListAsync();
    }
}
