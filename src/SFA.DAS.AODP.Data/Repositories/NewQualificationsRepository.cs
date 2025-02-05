using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories
{
    public class NewQualificationsRepository : INewQualificationsRepository
    {
        private static readonly List<NewQualification> MockNewQualifications = new()
        {
            new NewQualification { Id = 1, Title = "EDEXCEL Intermediate GNVQ in Business" },
            new NewQualification { Id = 2, Title = "OCR Intermediate GNVQ in Science" },
            new NewQualification { Id = 3, Title = "EDEXCEL Intermediate GNVQ in Art and Design" }
        };

        private static readonly List<QualificationDetails> MockQualificationDetails = new()
        {
            new QualificationDetails
            {
                Success = true,
                Id = 1,
                Status = "Decision required",
                Priority = "High",
                Changes = "Qualification title, Level",
                QualificationReference = "BUS123",
                AwardingOrganisation = "EDEXCEL",
                Title = "EDEXCEL Intermediate GNVQ in Business",
                QualificationType = "Vocational",
                Level = "3",
                ProposedChanges = "None",
                AgeGroup = "16-18",
                Category = "Business",
                Subject = "Commerce",
                SectorSubjectArea = "Business & Finance",
                Comments = "Requires funding review"
            }
        };

        public Task<List<NewQualification>> GetAllNewQualificationsAsync()
        {
            return Task.FromResult(MockNewQualifications);
        }

        public Task<QualificationDetails?> GetQualificationDetailsByIdAsync(int id)
        {
            var qualification = MockQualificationDetails.FirstOrDefault(q => q.Id == id);
            return Task.FromResult(qualification);
        }
    }
}
