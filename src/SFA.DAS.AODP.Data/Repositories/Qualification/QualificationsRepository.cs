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
        qualificationDiscussionHistory.ActionTypeId = await GetActionTypeId(qual.Id);
        _context.QualificationDiscussionHistory.Add(qualificationDiscussionHistory);
        await _context.SaveChangesAsync();
    }

    private async Task<Guid> GetActionTypeId(Guid qualificationId)
    {
        var lastItem = await _context.QualificationDiscussionHistory
            .OrderByDescending(v => v.Timestamp)
            .FirstOrDefaultAsync(v => v.QualificationId == qualificationId);
        if (lastItem is not null)
        {
            return lastItem.ActionTypeId;
        }
        return await _context.ActionType.Where(v => v.Description == "No Action Required")
            .Select(v => v.Id)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateQualificationStatus(string qualificationReference, Guid processStatusId,int version)
    {
        var qual = await _context.QualificationVersions
            .Include(v => v.LifecycleStage)
            .Include(v => v.Qualification)
            .FirstOrDefaultAsync(v => v.Qualification.Qan == qualificationReference && v.Version==version);
        if (qual is null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }
        if (!_context.ProcessStatus.Any(v => v.Id == processStatusId))
        {
            throw new NoForeignKeyException(processStatusId);
        }
        qual.ProcessStatusId = processStatusId;
        await _context.SaveChangesAsync();
    }

    public async Task<List<ProcessStatus>> GetProcessingStatuses() => await _context.ProcessStatus.ToListAsync();
    public async Task<IEnumerable<ChangedQualificationExport>> GetChangedQualificationsExport()
    {
        return await _context.ChangedQualificationExport.ToListAsync<ChangedQualificationExport>();
    }
}
