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

        public async Task<QualificationDetails?> GetQualificationDetailsByIdAsync(string qualificationReference)
        {
            var qualification = await _context.QualificationNewReviewRequired
                .Where(q => q.QualificationReference == qualificationReference)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (qualification == null)
            {
                return null;
            }

            var comments = await _context.QualificationDiscussionHistory
                .Where(v => v.Qualification.Qan == qualificationReference)
                .Include(v => v.ActionType)
                .Select(v => new Models.Qualifications.QualificationDiscussionHistory
                {
                    Id = v.Id,
                    QualificationId = v.QualificationId,
                    ActionTypeId = v.ActionTypeId,
                    UserDisplayName = v.UserDisplayName,
                    Notes = v.Notes,
                    Timestamp = v.Timestamp,
                    ActionType = new Models.Qualifications.ActionType
                    {
                        Id = v.ActionType.Id,
                        Description = v.ActionType.Description,
                    }
                })
                .ToListAsync();

            return new QualificationDetails
            {
                QualificationReference = qualification.QualificationReference,
                AwardingOrganisation = qualification.AwardingOrganisation,
                Title = qualification.QualificationTitle,
                QualificationType = qualification.QualificationType,
                Level = qualification.Level,
                AgeGroup = qualification.AgeGroup,
                Subject = qualification.Subject,
                SectorSubjectArea = qualification.SectorSubjectArea,
                Comments = "No comments available",

                // Placeholder values for missing properties
                Id = 1,
                Status = "New",
                Priority = "Medium",
                Changes = "No recent changes",
                ProposedChanges = "None",
                Category = "General Education",
                QualificationDiscussionHistories = comments
            };
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
    }
}


