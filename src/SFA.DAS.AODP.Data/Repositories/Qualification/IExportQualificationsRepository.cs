using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface IExportQualificationsRepository
    {
        Task<IEnumerable<QualificationExport>> GetQualificationExport();
    }
}