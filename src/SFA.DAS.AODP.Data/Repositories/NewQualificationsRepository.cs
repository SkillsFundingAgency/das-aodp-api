﻿using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Infrastructure.Context;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories
{
    public class NewQualificationsRepository : INewQualificationsRepository
    {
        private readonly ApplicationDbContext _context;

        public NewQualificationsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NewQualification>> GetAllNewQualificationsAsync()
        {
            var qualifications = await _context.QualificationNewReviewRequired.ToListAsync();

            return qualifications.Select(q => new NewQualification
            {
                Title = q.QualificationTitle,
                Reference = q.QualificationReference,
                AwardingOrganisation = q.AwardingOrganisation,
                Status = "New"
            }).ToList();
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

            return new QualificationDetails
            {
                Success = true,
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
                Status = "Pending Review", 
                Priority = "Medium", 
                Changes = "No recent changes", 
                ProposedChanges = "None", 
                Category = "General Education" 
            };
        }


    }
}


