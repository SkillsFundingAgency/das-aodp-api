using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Data.Repositories
{
    public interface IPageRepository
    {
        Task<List<Page>> GetPagesForSectionAsync(Guid sectionId);
        Task<Page?> GetPageByIdAsync(Guid pageId);
        Task<Page> Create(Page page);
        Task<Page?> Update(Page page);
        Task<Page?> Archive(Guid pageId);
        Task<List<Page>> CopyPagesForNewSection(Guid oldSectionId, Guid newSectionId);
    }
}
