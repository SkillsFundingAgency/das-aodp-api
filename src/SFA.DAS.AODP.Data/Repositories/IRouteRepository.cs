using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories
{
    public interface IRouteRepository
    {
        Task<List<View_AvailableQuestionsForRouting>> GetAvailableQuestionsForRoutingByPageId(Guid pageId);
        Task<List<View_AvailableQuestionsForRouting>> GetAvailableSectionsAndPagesForRoutingByFormVersionId(Guid formVersionId);
        Task<List<View_QuestionRoutingDetail>> GetQuestionRoutingDetailsByFormVersionId(Guid formVersionId);
        Task<List<Route>> GetRoutesByQuestionId(Guid questionId);
        Task UpsertAsync(List<Route> dbRoutes);
    }
}