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
        int GetMaxOrderBySectionId(Guid sectionId);
        Task<List<Page>> GetNextPagesInSectionByOrderAsync(Guid sectionId, int order);
        Task<Page> GetPageForApplicationAsync(int pageOrder, Guid sectionId);
        Task<List<Guid>> GetPagesIdInSectionByOrderAsync(Guid sectionId, int startOrder, int? endOrder);
        Task<List<Guid>> GetPagesIdInFormBySectionOrderAsync(Guid formVersionId, int startSectionOrder, int? endSectionOrder);
        Task<Dictionary<Guid, Guid>> CopyPagesForNewFormVersion(Dictionary<Guid, Guid> oldNewSectionIds);
        /// <summary>
        /// Finds a question with a given Id, and finds the next section with a higher Order (so will appear lower in the list) and switches them. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<bool> MovePageOrderDown(Guid id);

        /// <summary>
        /// Finds a question with a given Id, and finds the next section with a lower Order (so will appear higher in the list) and switches them. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<bool> MovePageOrderUp(Guid id);
        Task<bool> IsPageEditable(Guid id);
    }
}
