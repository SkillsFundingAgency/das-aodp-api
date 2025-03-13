using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface INewQualificationsRepository
    {
        Task<NewQualificationsResult> GetAllNewQualificationsAsync(int? skip = 0, int? take = 0, QualificationsFilter? filter = default);
        Task<QualificationVersions> GetQualificationDetailsByIdAsync(string qualificationReference);
        Task<List<QualificationExport>> GetNewQualificationsCSVExport();
        Task AddQualificationDiscussionHistory(Entities.Qualification.QualificationDiscussionHistory qualificationDiscussionHistory, string qualificationReference);
        Task UpdateQualificationStatus(string qualificationReference, string status);
    }
}