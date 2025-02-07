﻿using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IQuestionRepository
    {
        Task Archive(Guid questionId);
        Task<Dictionary<Guid, Guid>> CopyQuestionsForNewFormVersion(Dictionary<Guid, Guid> oldNewPageIds);
        Task<Question> Create(Question question);
        int GetMaxOrderByPageId(Guid pageId);
        Task<Question> GetQuestionByIdAsync(Guid id);
        Task<Question> GetQuestionDetailForRoutingAsync(Guid questionId);
        Task<string?> GetQuestionTypeById(Guid questionid);
        Task<Question> Update(Question question);
        Task ValidateQuestionForChange(Guid questionId);
    }
}