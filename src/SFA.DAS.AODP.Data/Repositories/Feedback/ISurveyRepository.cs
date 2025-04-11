using SFA.DAS.AODP.Data.Entities.Feedback;

namespace SFA.DAS.AODP.Data.Repositories.Feedback
{
    public interface ISurveyRepository
    {
        Task Create(Survey survey);
    }
}