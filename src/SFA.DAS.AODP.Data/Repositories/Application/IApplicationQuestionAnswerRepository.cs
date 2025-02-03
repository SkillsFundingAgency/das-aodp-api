using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationQuestionAnswerRepository
    {
        Task<ApplicationQuestionAnswer> Create(ApplicationQuestionAnswer application);
        Task<ApplicationQuestionAnswer> Update(ApplicationQuestionAnswer application);
        Task UpsertAsync(List<ApplicationQuestionAnswer>? questionAnswers);
    }
}