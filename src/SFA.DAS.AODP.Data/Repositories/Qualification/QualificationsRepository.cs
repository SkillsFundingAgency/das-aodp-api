using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public class QualificationsRepository : IQualificationsRepository
    {
        private readonly ApplicationDbContext _context;

        public QualificationsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Qualifications.Qualification>> GetAllNewQualificationsAsync() => await _context.QualificationNewReviewRequired
                      .Select(q => new Models.Qualifications.Qualification
                      {
                          Title = q.QualificationTitle,
                          Reference = q.QualificationReference,
                          AwardingOrganisation = q.AwardingOrganisation,
                          Status = "New"
                      })
                      .ToListAsync();

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
                Category = "General Education"
            };
        }

        public async Task<List<QualificationExport>> GetNewQualificationsCSVExport() =>
            await _context.NewQualificationCSVExport.ToListAsync();

        public async Task<List<Models.Qualifications.Qualification>> GetAllChangedQualificationsAsync()
        {
           return await _context.QualificationChangedReviewRequired
                      .Select(q => new Models.Qualifications.Qualification
                      {
                          Title = q.QualificationTitle,
                          Reference = q.QualificationReference,
                          AwardingOrganisation = q.AwardingOrganisation,
                          Status = "Changed"
                      })
                      .ToListAsync();
        }
    }
}


