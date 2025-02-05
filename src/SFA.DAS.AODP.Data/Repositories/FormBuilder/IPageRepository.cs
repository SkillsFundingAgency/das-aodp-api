using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IPageRepository
    {
        Task<List<Page>> GetPagesForSectionAsync(Guid sectionId);
        Task<Page> GetPageByIdAsync(Guid pageId);
        Task<Page> Create(Page page);
        Task<Page?> Update(Page page);
        Task<Page?> Archive(Guid pageId);
        Task<List<Page>> CopyPagesForNewSection(Guid oldSectionId, Guid newSectionId);
        int GetMaxOrderBySectionId(Guid sectionId);
        Task<List<Page>> GetNextPagesInSectionByOrderAsync(Guid sectionId, int order);
        Task<Page> GetPageForApplicationAsync(int pageOrder, Guid sectionId);
        Task<List<Guid>> GetPagesIdInSectionByOrderAsync(Guid sectionId, int startOrder, int? endOrder);
        Task<List<Guid>> GetPagesIdInFormBySectionOrderAsync(Guid formVersionId, int startSectionOrder, int? endSectionOrder);
    }
}
