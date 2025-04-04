
namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface ISurveyRepository
    {
        Task Create(Data.Entities.Application.Survey survey);
    }
}