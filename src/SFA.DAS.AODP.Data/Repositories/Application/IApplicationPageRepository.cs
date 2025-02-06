﻿using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationPageRepository
    {
        Task<ApplicationPage> Create(ApplicationPage application);
        Task<ApplicationPage?> GetApplicationPageByPageIdAsync(Guid applicationId, Guid pageId);
        Task<List<ApplicationPage>> GetApplicationPagesByPageIdsAsync(Guid applicationId, List<Guid> pageIds);
        Task<List<ApplicationPage>> GetBySectionIdAsync(Guid sectionId, Guid applicationId);
        Task<List<ApplicationPage>> GetSkippedApplicationPagesByQuestionIdAsync(Guid applicationId, Guid questionId, List<Guid> pageIdsToIgnore);
        Task<ApplicationPage> Update(ApplicationPage application);
        Task UpsertAsync(ApplicationPage page);
        Task UpsertAsync(List<ApplicationPage> applicationPagesToUpsert);
    }
}