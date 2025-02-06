using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder;

public interface ISectionRepository
{
    /// <summary>
    /// Gets all sections for a given form version Id. 
    /// Does not check if the form version Id is valid, will just return an empty list if so. 
    /// </summary>
    /// <param name="formVersionId"></param>
    /// <returns></returns>
    Task<List<Section>> GetSectionsForFormAsync(Guid formId);

    /// <summary>
    /// Gets a section with a given Id. 
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    Task<Section> GetSectionByIdAsync(Guid sectionId);

    /// <summary>
    /// Creates a new section, throws if the form version Id is not found. 
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    /// <exception cref="NoForeignKeyException"></exception>
    Task<Section> Create(Section section);

    /// <summary>
    /// Updates a section's data, throws if no section for the given Id can be found, 
    /// or if the linked form version is not in draft. 
    /// </summary>
    /// <param name="section"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    /// <exception cref="RecordLockedException"></exception>
    Task<Section> Update(Section section);

    /// <summary>
    /// Deletes a section with a given Id, throws if no record can be found for a given Id. 
    /// </summary>
    /// <param name="sectionId"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    Task<Section> DeleteSection(Guid sectionId);

    /// <summary>
    /// Copies all sections associated with one form version id to another. 
    /// Used when creating a new form version from an old one. 
    /// </summary>
    /// <param name="oldFormVersionId"></param>
    /// <param name="newFormVersionId"></param>
    /// <returns></returns>
    Task<List<Section>> CopySectionsForNewForm(Guid oldFormId, Guid newFormId);
    int GetMaxOrderByFormVersionId(Guid formVersionId);
    Task<List<Section>> GetSectionsByIdAsync(List<Guid> sectionIds);
    Task<List<Section>> GetNextSectionsByOrderAsync(Guid formVersionId, int order);
}
