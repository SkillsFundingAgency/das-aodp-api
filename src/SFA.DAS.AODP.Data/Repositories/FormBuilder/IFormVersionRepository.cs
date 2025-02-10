﻿using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IFormVersionRepository
    {
        /// <summary>
        /// Sets the status of a form version with a given Id to the status of archived. 
        /// </summary>
        /// <param name="formVersionId"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<bool> Archive(Guid formVersionId);

        /// <summary>
        /// Creates a new form and a related form version from a passed in form version. 
        /// </summary>
        /// <param name="formVersionToAdd"></param>
        /// <returns></returns>
        Task<FormVersion> Create(FormVersion formVersionToAdd);

        /// <summary>
        /// Gets a form version by its DB Id. 
        /// </summary>
        /// <param name="formVersionId"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<FormVersion> GetFormVersionByIdAsync(Guid formVersionId);

        /// <summary>
        /// Returns all the latest form versions for all given forms. 
        /// </summary>
        /// <returns></returns>
        Task<List<FormVersion>> GetLatestFormVersions();

        /// <summary>
        /// Updates a given form version using the data and DB Id from a passed in form model. 
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<FormVersion> Update(FormVersion form);

        /// <summary>
        /// Sets the status of a form version with the given ID and with the current status of "Draft" to "Published". 
        /// </summary>
        /// <param name="formVersionId"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<bool> Publish(Guid formVersionId);

        /// <summary>
        /// Sets the status of a form version with the given ID to "Archived". 
        /// </summary>
        /// <param name="formVersionId"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<bool> Unpublish(Guid formVersionId);
        Task<List<FormVersion>> GetPublishedFormVersions();
        int GetMaxOrder();
        Task<FormVersion?> GetDraftFormVersionByFormId(Guid formId);
        Task<FormVersion?> GetPublishedFormVersionByFormId(Guid formId);
        Task<FormVersion> CreateDraftAsync(Guid publishedFormVersionId);
        Task<bool> IsFormVersionEditable(Guid formVersionId);
        /// <summary>
        /// Finds a form version with a given Id, and finds the next section with a higher Order (so will appear lower in the list) and switches them. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<bool> MoveFormVersionOrderDown(Guid id);

        /// <summary>
        /// Finds a form version with a given Id, and finds the next section with a lower Order (so will appear higher in the list) and switches them. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        Task<bool> MoveFormVersionOrderUp(Guid id);
    }
}