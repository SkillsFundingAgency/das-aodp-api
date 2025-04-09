﻿using Microsoft.EntityFrameworkCore;
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
            .Include(v => v.ProcessStatus)
            .Include(v => v.Organisation)
            .Include(v => v.VersionFieldChanges)
            .Include(v => v.Qualification)
            .OrderByDescending(v => v.Version)
            .FirstOrDefaultAsync(v => v.Qualification.Qan == qualificationReference);

        if (qualVersion == null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }

        return qualVersion;
    }

    public async Task<QualificationVersions> GetVersionByIdAsync(string qualificationReference, int version)
    {
        var qualVersion = await _context.QualificationVersions
            .Include(v => v.LifecycleStage)
            .Include(v => v.ProcessStatus)
            .Include(v => v.Organisation)
            .Include(v => v.VersionFieldChanges)
            .Include(v => v.Qualification)
            .OrderByDescending(v => v.Version)
            .FirstOrDefaultAsync(v => v.Qualification.Qan == qualificationReference && v.Version == version);

        if (qualVersion == null)
        {
            throw new RecordWithNameNotFoundException(qualificationReference);
        }

        return qualVersion;
    }
    public async Task<List<QualificationDiscussionHistory>> GetDiscussionHistoriesForQualificationRef(string qualificationRef)
    {
        return await _context.QualificationDiscussionHistory.Where(v => v.Qualification.Qan == qualificationRef)
            .Include(v => v.ActionType)
            .ToListAsync();
    }
}
