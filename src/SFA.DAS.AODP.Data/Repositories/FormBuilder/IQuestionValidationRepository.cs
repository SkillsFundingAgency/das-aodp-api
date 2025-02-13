using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IQuestionValidationRepository
    {
        Task CopyQuestionValidationForNewFormVersion(Dictionary<Guid, Guid> oldNewQuestionIds);
        Task<QuestionValidation?> GetQuestionValidationsByQuestionIdAsync(Guid questionId);
        Task UpsertAsync(QuestionValidation validation);
    }
}
