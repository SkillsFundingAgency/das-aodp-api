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
    int GetMaxOrderByFormVersionId(Guid formVersionId);
    Task<List<Section>> GetSectionsByIdAsync(List<Guid> sectionIds);
    Task<Section> GetSectionByIdWithPagesAndQuestionsAsync(Guid sectionId);
    Task<List<Section>> GetNextSectionsByOrderAsync(Guid formVersionId, int order);
    Task<Dictionary<Guid,Guid>> CopySectionsForNewFormVersion(Guid oldFormVersionId, Guid newFormVersionId);
    /// <summary>
    /// Finds a section with a given Id, and finds the next section with a lower Order (so will appear higher in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    Task<bool> MoveSectionOrderUp(Guid sectionId);

    /// <summary>
    /// Finds a section with a given Id, and finds the next section with a higher Order (so will appear lower in the list) and switches them. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException"></exception>
    Task<bool> MoveSectionOrderDown(Guid id);
    Task<bool> IsSectionEditable(Guid id);

    /// <summary>
    /// Checks if a section has any associated routes directly or indirectly via related Pages or Questions
    /// </summary>
    /// <param name="sectionId">The unique identifier of the section.</param>
    /// <returns>A boolean value indicating whether the section has associated routes.</returns>
    Task<bool> HasRoutesForSectionAsync(Guid sectionId);

    /// <summary>
    /// Returns Sections with associated Pages, Questions and Question Options (aimed at Form Preview)
    /// </summary>
    /// <param name="formVersionId">The unique identifier of the Form Version.</param>
    /// <returns></returns>
    Task<List<Section>> GetSectionsWithPagesAndQuestionsByFormVersionIdAsync(Guid formVersionId);
}
