using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IQuestionOptionRepository
    {
        Task<Dictionary<Guid, Guid>> CopyQuestionOptionsForNewFormVersion(Dictionary<Guid, Guid> oldNewQuestionIds);
        Task RemoveAsync(List<QuestionOption> optionsToRemove);
        Task UpsertAsync(List<QuestionOption> options);
    }
}