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
        qualificationDiscussionHistory.Timestamp = DateTime.UtcNow;
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

    public async Task<QualificationVersions?> GetQualificationVersionByQanAsync(string qualificationReference, CancellationToken cancellationToken) 
        => await _context.QualificationVersions
            .Include(o => o.ProcessStatus)
            .Include(o => o.LifecycleStage)
            .Include(o => o.Qualification)
            .FirstOrDefaultAsync(x => x.Qualification.Qan == qualificationReference, cancellationToken: cancellationToken);

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

    public async Task<List<SearchedQualification>> GetSearchedQualificationByNameAsync(string searchTerm)
    {
        var pattern = $"%{searchTerm.Trim()}%";

        return await _context.QualificationFundingStatus
            .AsNoTracking()
            .Where(q => EF.Functions.Like((q.QualificationName ?? string.Empty), pattern))
            .OrderBy(q => q.FundingStatus)
            .ThenBy(q => q.AwardingOrganisationName)
            .ThenBy(q => q.QualificationName)
            .Select(q => new SearchedQualification
            {
                Id = q.QualificationId,
                QualificationName = q.QualificationName,
                Qan = q.Qan!,
                Status = q.FundedStatus
            })
            .ToListAsync();
    }
}
