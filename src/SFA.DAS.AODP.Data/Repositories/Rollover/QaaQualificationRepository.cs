using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

/// <summary>
/// Implementation for <see cref="IQaaQualificationRepository"/>.
/// </summary>
/// <param name="context">The context to manage entities.</param>
public class QaaQualificationRepository(ApplicationDbContext context) : IQaaQualificationRepository
{
    private readonly ApplicationDbContext _context = context;

    /// <inheritdoc/>.
    public async Task<IEnumerable<RegulatedQaaQualification>> GetAllAsync(CancellationToken cancellationToken) 
        => await _context.RegulatedQaaQualifications.ToListAsync(cancellationToken);
}