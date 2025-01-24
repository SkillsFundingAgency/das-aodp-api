using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories;

public interface IPageRepository
{
    /// <summary>
    /// Gets all pages for a given section. 
    /// Does not check if the section Id is valid, retuns an empty list if so.  
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    Task<List<Page>> GetPagesForSectionAsync(Guid sectionId);

    /// <summary>
    /// Gets a page with a given Id, throws if no page is found with the given Id. 
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    Task<Page> GetPageByIdAsync(Guid pageId);

    /// <summary>
    /// Creates a page on a section, throws if no linked section is found, 
    /// or if the linked form version isn't in draft status. 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    /// <exception cref="NoForeignKeyException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    Task<Page> Create(Page page);

    /// <summary>
    /// Updates a page, throws is no page with a given Id is found, 
    /// or the linked form version isn't in draft. 
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    Task<Page> Update(Page page);

    /// <summary>
    /// Archives a page, throws is no page with a given Id is found, 
    /// or the linked form version isn't in draft. 
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    Task<Page> Archive(Guid pageId);

    /// <summary>
    /// Copies all pages with a given section id, to a new section id. 
    /// Used when creating a new form version from an old one. 
    /// </summary>
    /// <param name="oldSectionId"></param>
    /// <param name="newSectionId"></param>
    /// <returns></returns>
    Task<List<Page>> CopyPagesForNewSection(Guid oldSectionId, Guid newSectionId);
}
