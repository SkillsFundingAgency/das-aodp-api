using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface INewQualificationsRepository
    {
        Task<NewQualificationsResult> GetAllNewQualificationsAsync(int? skip = 0, int? take = 0, NewQualificationsFilter? filter = default);
        Task<IEnumerable<NewQualificationExport>> GetNewQualificationsExport();
    }
}