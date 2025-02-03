using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IPageRepository
    {
        Task<List<Page>> GetPagesForSectionAsync(Guid sectionId);
        Task<Page?> GetPageByIdAsync(Guid pageId);
        Task<Page> Create(Page page);
        Task<Page?> Update(Page page);
        Task<Page?> Archive(Guid pageId);
        Task<List<Page>> CopyPagesForNewSection(Guid oldSectionId, Guid newSectionId);
        int GetMaxOrderBySectionId(Guid sectionId);
        Task<List<Page>> GetNextPagesInSectionByOrderAsync(Guid sectionId, int order);
        Task<Page> GetPageForApplicationAsync(int pageOrder, Guid sectionId);
    }
}
