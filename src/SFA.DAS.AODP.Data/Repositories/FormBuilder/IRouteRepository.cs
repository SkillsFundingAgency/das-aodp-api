using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IRouteRepository
    {
        Task CopyRoutesForNewFormVersion(Dictionary<Guid, Guid> oldNewQuestionIds, Dictionary<Guid, Guid> oldNewPageIds, Dictionary<Guid, Guid> oldNewSectionIds, Dictionary<Guid, Guid> oldNewOptionIds);
        Task<List<View_AvailableQuestionsForRouting>> GetAvailableQuestionsForRoutingByPageId(Guid pageId);
        Task<List<View_AvailableQuestionsForRouting>> GetAvailableSectionsAndPagesForRoutingByFormVersionId(Guid formVersionId);
        Task<List<View_QuestionRoutingDetail>> GetQuestionRoutingDetailsByFormVersionId(Guid formVersionId);
        Task<List<Route>> GetRoutesByQuestionId(Guid questionId);
        Task UpsertAsync(List<Route> dbRoutes);
    }
}