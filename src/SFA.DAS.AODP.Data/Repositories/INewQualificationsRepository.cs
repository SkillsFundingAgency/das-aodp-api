using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories
{
    public interface INewQualificationsRepository
    {
        Task<List<NewQualification>> GetAllNewQualificationsAsync();
        Task<QualificationDetails?> GetQualificationDetailsByIdAsync(int id);
    }
}