using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
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
                      QualificationReference= q.QualificationReference,
                      AwardingOrganisation = q.AwardingOrganisation,
                      Status = q.Status,
                      AgeGroup = q.AgeGroup
                  }).ToList();

        return new ChangedQualificationsResult()
        {
            Data = records,
            Skip = skip,
            Take = take,
            TotalRecords = totalRecords
        };
    }
}
