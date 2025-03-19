using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using ChangedQualification = Entities.Qualification.ChangedQualification;

public class QualificationsRepository(ApplicationDbContext context) : IQualificationsRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<ChangedQualification>> GetChangedQualificationsAsync()
    {
        return await _context.ChangedQualifications
            .ToListAsync();
    }
    public async Task AddQualificationDiscussionHistory(Entities.Qualification.QualificationDiscussionHistory qualificationDiscussionHistory, string qualificationReference)
    {
        var qual = await _context.Qualification.FirstOrDefaultAsync(v => v.Qan == qualificationReference);
        if (qual is null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }
        qualificationDiscussionHistory.QualificationId = qual.Id;
        qualificationDiscussionHistory.Timestamp = DateTime.Now;
        _context.QualificationDiscussionHistory.Add(qualificationDiscussionHistory);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateQualificationStatus(string qualificationReference, string status)
    {
        var qual = await _context.QualificationVersions
            .Include(v => v.LifecycleStage)
            .Include(v => v.Qualification)
            .FirstOrDefaultAsync(v => v.Qualification.Qan == qualificationReference);
        if (qual is null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }
        qual.Status = status;
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<ChangedQualificationExport>> GetChangedQualificationsExport()
    {
        return await _context.ChangedQualificationExport.ToListAsync<ChangedQualificationExport>();
    }
}
