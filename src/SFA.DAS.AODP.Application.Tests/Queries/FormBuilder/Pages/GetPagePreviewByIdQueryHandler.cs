//using MediatR;
//using SFA.DAS.AODP.Application;
//using SFA.DAS.AODP.Data.Repositories.FormBuilder;

//namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

//public class GetPagePreviewByIdQueryHandler : IRequestHandler<GetPagePreviewByIdQuery, BaseMediatrResponse<GetPagePreviewByIdQueryResponse>>
//{
//    private readonly IPageRepository _pageRepository;

//    public GetPagePreviewByIdQueryHandler(IPageRepository pageRepository) => _pageRepository = pageRepository;

//    public async Task<BaseMediatrResponse<GetPagePreviewByIdQueryResponse>> Handle(GetPagePreviewByIdQuery request, CancellationToken cancellationToken)
//    {
//        var response = new BaseMediatrResponse<GetPagePreviewByIdQueryResponse>();
//        try
//        {
//            var page = await _pageRepository.GetPageByIdAsync(request.PageId);
//            var result = await _pageRepository.GetPageForApplicationAsync(page.Order, request.SectionId, request.FormVersionId);
//            response.Value = result;
//            response.Success = true;
//        }
//        catch (Exception ex)
//        {
//            response.ErrorMessage = ex.Message;
//        }

//        return response;
//    }
//}