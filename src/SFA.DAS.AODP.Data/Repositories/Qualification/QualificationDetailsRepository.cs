using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

public class QualificationDetailsRepository(IApplicationDbContext context) : IQualificationDetailsRepository
{
    private readonly IApplicationDbContext _context = context;

    public async Task<QualificationVersions> GetQualificationDetailsByIdAsync(string qualificationReference)
    {
        var qualVersion = await _context.QualificationVersions
            .Include(v => v.LifecycleStage)
            .Include(v => v.Organisation)
            .Include(v => v.VersionFieldChanges)
            .Include(v => v.Qualification)
                .ThenInclude(v => v.QualificationDiscussionHistories)
                    .ThenInclude(v => v.ActionType)
            .OrderByDescending(v => v.Version)
            .FirstOrDefaultAsync(v => v.LifecycleStage.Name == "New" && v.Qualification.Qan == qualificationReference);

        if (qualVersion == null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }

        return qualVersion;
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
            .FirstOrDefaultAsync(v => v.LifecycleStage.Name == "New" && v.Qualification.Qan == qualificationReference);
        if (qual is null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }
        qual.Status = status;
        await _context.SaveChangesAsync();
    }
}
