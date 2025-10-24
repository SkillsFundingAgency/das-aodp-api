using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface IQualificationOutputFileRepository
    {
        Task<IEnumerable<QualificationOutputFile>> GetQualificationOutputFile();
    }
}