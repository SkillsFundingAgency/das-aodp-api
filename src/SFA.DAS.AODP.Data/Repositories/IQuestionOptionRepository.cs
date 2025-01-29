using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Data.Repositories
{
    public interface IQuestionOptionRepository
    {
        Task RemoveAsync(List<QuestionOption> optionsToRemove);
        Task UpsertAsync(List<QuestionOption> options);
    }
}