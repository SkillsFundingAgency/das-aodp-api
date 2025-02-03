using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationPageRepository
    {
        Task<ApplicationPage> Create(ApplicationPage application);
        Task<ApplicationPage?> GetApplicationPageByPageIdAsync(Guid applicationId, Guid pageId);
        Task<ApplicationPage> Update(ApplicationPage application);
        Task UpsertAsync(ApplicationPage page);
    }
}