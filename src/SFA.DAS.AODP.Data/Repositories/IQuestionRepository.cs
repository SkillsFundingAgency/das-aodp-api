using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories
{
    public interface IQuestionRepository
    {
        Task Archive(Guid questionId);
        Task<Question> Create(Question question);
        int GetMaxOrderByPageId(Guid pageId);
        Task<Question> GetQuestionByIdAsync(Guid id);
        Task<Question> GetQuestionDetailForRoutingAsync(Guid questionId);
        Task<Question> Update(Question question);
        Task ValidateQuestionForChange(Guid questionId);
    }
}