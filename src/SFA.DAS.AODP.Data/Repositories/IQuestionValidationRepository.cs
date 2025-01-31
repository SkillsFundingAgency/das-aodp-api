using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories
{
    public interface IQuestionValidationRepository
    {
        Task<QuestionValidation?> GetQuestionValidationsByQuestionIdAsync(Guid questionId);
        Task UpsertAsync(QuestionValidation validation);
    }
}