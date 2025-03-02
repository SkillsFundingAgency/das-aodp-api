using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface IQualificationsRepository
    {
        Task<List<Models.Qualifications.Qualification>> GetAllNewQualificationsAsync();
        Task<QualificationDetails?> GetQualificationDetailsByIdAsync(string qualificationReference);
        Task<List<QualificationExport>> GetNewQualificationsCSVExport();
        Task<List<Models.Qualifications.Qualification>> GetAllChangedQualificationsAsync();

    }
}