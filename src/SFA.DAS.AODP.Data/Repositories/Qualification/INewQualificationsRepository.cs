using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface INewQualificationsRepository
    {
        Task<NewQualificationsResult> GetAllNewQualificationsAsync(int? skip = 0, int? take = 0, QualificationsFilter? filter = default);
        Task<QualificationDetails?> GetQualificationDetailsByIdAsync(string qualificationReference);
        Task<List<QualificationExport>> GetNewQualificationsCSVExport();
    }
}