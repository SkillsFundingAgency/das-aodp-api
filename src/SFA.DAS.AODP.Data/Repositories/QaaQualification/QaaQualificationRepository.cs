using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.QaaQualification;

namespace SFA.DAS.AODP.Data.Repositories.QaaQualification;

/// <summary>
/// Implementation for <see cref="IQaaQualificationRepository"/>.
/// </summary>
/// <param name="context">The context to manage entities.</param>
public class QaaQualificationRepository(ApplicationDbContext context) : IQaaQualificationRepository
{
    private readonly ApplicationDbContext _context = context;

    /// <inheritdoc/>.
    public async Task<IEnumerable<RegulatedQaaQualification>> GetAllAsync(CancellationToken cancellationToken) 
        => await _context.RegulatedQaaQualifications
            .Include(q => q.Fundings)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>.
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc/>.
    public async Task<QaaQualificationSummaryCounts> GetSummaryCountsAsync(CancellationToken cancellationToken)
    {
        var query = _context.RegulatedQaaQualifications.AsNoTracking();

        return new QaaQualificationSummaryCounts
        {
            DataLastImportedDate = await query.Select(q => (DateTime?)q.DateOfDataSnapshot).MaxAsync(cancellationToken),
            NewCount = await query.CountAsync(q => q.LatestImportComparisonOutcome == QaaImportComparisonOutcome.New, cancellationToken),
            ExtendedCount = await query.CountAsync(q =>
                q.LatestImportComparisonOutcome == QaaImportComparisonOutcome.LastDateForRegistrationChanged &&
                q.LastDateForRegistrationChangeType == QaaLastDateForRegistrationChangeType.Extended, cancellationToken),
            DiscontinuedCount = await query.CountAsync(q => q.IsDiscontinued, cancellationToken)
        };
    }
}
