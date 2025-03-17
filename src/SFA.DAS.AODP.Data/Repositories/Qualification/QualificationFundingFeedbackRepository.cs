using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

public class QualificationFundingFeedbackRepository : IQualificationFundingFeedbackRepository
{
    private readonly IApplicationDbContext _context;

    public QualificationFundingFeedbackRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<QualificationFundingFeedbacks> CreateAsync(QualificationFundingFeedbacks QualificationFundingFeedback)
    {
        QualificationFundingFeedback.Id = Guid.NewGuid();

        await _context.QualificationFundingFeedbacks.AddAsync(QualificationFundingFeedback);
        await _context.SaveChangesAsync();

        return QualificationFundingFeedback;
    }

    public async Task UpdateAsync(QualificationFundingFeedbacks QualificationFundingFeedback)
    {
        _context.QualificationFundingFeedbacks.Update(QualificationFundingFeedback);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(List<QualificationFundingFeedbacks> QualificationFundingFeedbacks)
    {
        _context.QualificationFundingFeedbacks.UpdateRange(QualificationFundingFeedbacks);
        await _context.SaveChangesAsync();
    }
    public async Task<QualificationFundingFeedbacks> GetQualificationFundingFeedbackDetailsByIdAsync(Guid QualificationVersionId)
    {
        return await _context.QualificationFundingFeedbacks
                             .Include(a => a.QualificationVersion)
                             .FirstOrDefaultAsync(v => v.QualificationVersionId == QualificationVersionId);
    }
}
