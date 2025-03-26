//using MediatR;
//using SFA.DAS.AODP.Application.Exceptions;
//using SFA.DAS.AODP.Data.Exceptions;
//using SFA.DAS.AODP.Data.Repositories.FormBuilder;

//namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

//public class GetPageByIdQueryHandler(IPageRepository _pageRepository, IFormVersionRepository _formVersionRepository, IRouteRepository _routeRepository) : IRequestHandler<GetPageByIdQuery, BaseMediatrResponse<GetPageByIdQueryResponse>>
//{
//    public async Task<BaseMediatrResponse<GetPageByIdQueryResponse>> Handle(GetPageByIdQuery request, CancellationToken cancellationToken)
//    {
//        var response = new BaseMediatrResponse<GetPageByIdQueryResponse>();
//        var hasAssociatedRoutes = false;

//        try
//        {
//            var page = await _pageRepository.GetPageByIdAsync(request.PageId);

//            // Check routes for Page Directly
//            var routesForPage = await _routeRepository.GetRoutesByPageId(request.PageId);
//            if (routesForPage != null && routesForPage.Any())
//            {
//                hasAssociatedRoutes = true;
//            }

//            //Check Routes for related questions
//            foreach (var question in page.Questions)
//            {
//                var routesForRelatedQuestions = await _routeRepository.GetRoutesByQuestionId(question.Id);
//                if (routesForRelatedQuestions != null && routesForRelatedQuestions.Any())
//                {
//                    hasAssociatedRoutes = true;
//                }
//            }
//            response.Value = page;
//            response.Value.HasAssociatedRoutes = hasAssociatedRoutes;
//            response.Value.Editable = await _formVersionRepository.IsFormVersionEditable(request.FormVersionId);
//            response.Success = true;
//        }
//        catch (RecordNotFoundException ex)
//        {
//            response.Success = false;
//            response.InnerException = new NotFoundException(ex.Id);
//        }
//        catch (Exception ex)
//        {
//            response.ErrorMessage = ex.Message;
//        }

//        return response;
//    }
//}
