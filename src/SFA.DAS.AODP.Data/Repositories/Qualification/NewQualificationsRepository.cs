using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Enum;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public class NewQualificationsRepository : INewQualificationsRepository
    {
        private readonly IApplicationDbContext _context;

        public NewQualificationsRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NewQualificationsResult> GetAllNewQualificationsAsync(int? skip = 0, int? take = 0, QualificationsFilter? filter = default)
        {
            var query = _context.QualificationNewReviewRequired.AsQueryable();
            var countQuery = _context.QualificationNewReviewRequired.AsQueryable();

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
                      .Select(q => new NewQualification
                      {
                          Title = q.QualificationTitle,
                          Reference = q.QualificationReference,
                          AwardingOrganisation = q.AwardingOrganisation,
                          Status = "New",
                          AgeGroup = q.AgeGroup                          
                      }).ToList();

            return new NewQualificationsResult()
            {
                Data = records,
                Skip = skip,
                Take = take,
                TotalRecords = totalRecords
            };
        }

        public async Task<QualificationVersions?> GetQualificationDetailsByIdAsync(string qualificationReference)
        {
            var qualVersion = await _context.QualificationVersions
                .OrderByDescending(v => v.Version)
                .Include(v => v.LifecycleStage)
                .Include(v => v.Organisation)
                .Include(v => v.Qualification)
                    .ThenInclude(v => v.QualificationDiscussionHistories)
                    .ThenInclude(v => v.ActionType)
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

        public async Task<List<QualificationExport>> GetNewQualificationsCSVExport() =>
            await _context.NewQualificationCSVExport.ToListAsync();

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
}


