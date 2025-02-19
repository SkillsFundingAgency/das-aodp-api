using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories.FormBuilder
{
    public interface IRouteRepository
    {
        Task CopyRoutesForNewFormVersion(Dictionary<Guid, Guid> oldNewQuestionIds, Dictionary<Guid, Guid> oldNewPageIds, Dictionary<Guid, Guid> oldNewSectionIds, Dictionary<Guid, Guid> oldNewOptionIds);
        Task<List<View_AvailableQuestionsForRouting>> GetAvailableQuestionsForRoutingByPageId(Guid pageId);
        Task<List<View_AvailableQuestionsForRouting>> GetAvailableSectionsAndPagesForRoutingByFormVersionId(Guid formVersionId);
        Task<List<View_QuestionRoutingDetail>> GetQuestionRoutingDetailsByFormVersionId(Guid formVersionId);
        Task<List<View_QuestionRoutingDetail>> GetQuestionRoutingDetailsByQuestionId(Guid questionId);
        Task<List<Route>> GetRoutesByQuestionId(Guid questionId);
        Task<List<Route>> GetRoutesByPageId(Guid pageId);
        Task<bool> IsRouteEditable(Guid id);
        Task UpsertAsync(List<Route> dbRoutes);
    }
}