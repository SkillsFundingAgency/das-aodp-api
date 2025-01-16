using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Data.Repositories
{
    public interface IFormRepository
    {
        Task<List<FormVersion>> GetLatestFormVersions();
    }
}