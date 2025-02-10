using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface INewQualificationsRepository
    {
        Task<List<NewQualification>> GetAllNewQualificationsAsync();
        Task<QualificationDetails?> GetQualificationDetailsByIdAsync(string qualificationReference);
    }
}