using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Data.Repositories
{
    public interface IFormVersionRepository
    {
        Task<bool> Archive(Guid formVersionId);
        Task<FormVersion> Create(FormVersion formVersionToAdd);
        Task<FormVersion?> GetFormVersionByIdAsync(Guid formVersionId);
        Task<List<FormVersion>> GetLatestFormVersions();
        Task<FormVersion?> Update(FormVersion form);
        Task<bool> Publish(Guid formVersionId);
        Task<bool> Unpublish(Guid formVersionId);
    }
}