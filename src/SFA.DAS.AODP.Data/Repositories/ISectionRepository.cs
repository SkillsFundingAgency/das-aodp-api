using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Data.Repositories
{
    public interface ISectionRepository
    {
        Task<List<Section>> GetSectionsForFormAsync(Guid formId);
        Task<Section?> GetSectionByIdAsync(Guid sectionId);
        Task<Section> Create(Section section);
        Task<Section?> Update(Section section);
        Task<Section?> ArchiveSection(Guid sectionId);
        Task<List<Section>> CopySectionsForNewForm(Guid oldFormId, Guid newFormId);
    }
}
