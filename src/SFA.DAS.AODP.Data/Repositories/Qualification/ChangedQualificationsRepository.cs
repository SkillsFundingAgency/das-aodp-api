﻿using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;


public class ChangedQualificationsRepository(ApplicationDbContext context) : IChangedQualificationsRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<ChangedQualificationsResult> GetAllChangedQualificationsAsync(int? skip = 0, int? take = 0, QualificationsFilter? filter = default)
    {
        var query = _context.ChangedQualifications.AsQueryable();
        var countQuery = _context.ChangedQualifications.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter?.Name))
        {
            query = query.Where(w => w.QualificationTitle.Contains(filter.Name));
            countQuery = countQuery.Where(w => w.QualificationTitle.Contains(filter.Name));
        }

        if (!string.IsNullOrWhiteSpace(filter?.Organisation))
        {
            query = query.Where(w => w.AwardingOrganisation.Contains(filter.Organisation));
            countQuery = countQuery.Where(w => w.AwardingOrganisation.Contains(filter.Organisation));
        }

        if (!string.IsNullOrWhiteSpace(filter?.QAN))
        {
            query = query.Where(w => w.QualificationReference.Equals(filter.QAN));
            countQuery = countQuery.Where(w => w.QualificationReference.Equals(filter.QAN));
        }

        if (filter?.ProcessStatusIds?.Any() ?? false)
        {
            query = query.Where(w => filter.ProcessStatusIds.Contains(w.ProcessStatusId ?? Guid.Empty));
            countQuery = countQuery.Where(w => filter.ProcessStatusIds.Contains(w.ProcessStatusId ?? Guid.Empty));
        }

        query = query.OrderBy(o => o.QualificationTitle);
        var totalRecords = await countQuery.CountAsync();

        var skipChecked = skip ?? 0;
        var takeChecked = take ?? 500;
        var executed = await query
                    .Skip(skipChecked)
                    .Take(takeChecked)
                    .ToListAsync();

        var records = executed
                  .Select(q => new DAS.AODP.Models.Qualifications.ChangedQualification
                  {
                      QualificationTitle = q.QualificationTitle,
                      QualificationReference = q.QualificationReference,
                      AwardingOrganisation = q.AwardingOrganisation,
                      ChangedFieldNames = q.ChangedFieldNames,
                      SectorSubjectArea = q.SectorSubjectArea,
                      Level = q.Level,
                      QualificationType = q.QualificationType, 
                      Subject = q.Subject,                      
                      AgeGroup = q.AgeGroup,
                      Status = q.Status
                  }).ToList();

        return new ChangedQualificationsResult()
        {
            Data = records,
            Skip = skip,
            Take = take,
            TotalRecords = totalRecords
        };
    }

    public async Task<IEnumerable<ChangedQualificationExport>> GetChangedQualificationsCSVExport()
    {
        return await _context.ChangedQualificationExport.ToListAsync<ChangedQualificationExport>();
    }

    public async Task<List<Entities.Qualification.ActionType>> GetActionTypes()
    {
        return await _context.ActionType.ToListAsync();
    }
}
