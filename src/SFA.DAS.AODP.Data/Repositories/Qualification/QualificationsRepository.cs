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
    public async Task AddQualificationDiscussionHistory(QualificationDiscussionHistory qualificationDiscussionHistory, string qualificationReference)
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

    public async Task<ProcessStatus> UpdateQualificationStatus(string qualificationReference, Guid processStatusId, int? version)
    {
        var query = _context.QualificationVersions.Include(v => v.LifecycleStage)
            .Include(v => v.Qualification)
            .OrderByDescending(v => v.Version);
        var qual = version is not null ?
            await query.FirstOrDefaultAsync(v => v.Qualification.Qan == qualificationReference && v.Version == version) :
            await query.FirstOrDefaultAsync(v => v.Qualification.Qan == qualificationReference);
        if (qual is null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }
        var processStatus = await _context.ProcessStatus.FirstOrDefaultAsync(v => v.Id == processStatusId);
        if (processStatus is null)
        {
            throw new NoForeignKeyException(processStatusId);
        }
        qual.ProcessStatusId = processStatusId;
        await _context.SaveChangesAsync();
        return processStatus;
    }

    public async Task<List<ProcessStatus>> GetProcessingStatuses() => await _context.ProcessStatus.ToListAsync();
    public async Task<IEnumerable<ChangedQualificationExport>> GetChangedQualificationsExport()
    {
        return await _context.ChangedQualificationExport.ToListAsync<ChangedQualificationExport>();
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
