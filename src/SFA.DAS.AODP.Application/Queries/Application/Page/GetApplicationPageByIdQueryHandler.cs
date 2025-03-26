using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class GetApplicationPageByIdQueryHandler : IRequestHandler<GetApplicationPageByIdQuery, BaseMediatrResponse<GetApplicationPageByIdQueryResponse>>
{
    private readonly IPageRepository _pageRepository;

    public GetApplicationPageByIdQueryHandler(IPageRepository pageRepository) => _pageRepository = pageRepository;

    public async Task<BaseMediatrResponse<GetApplicationPageByIdQueryResponse>> Handle(GetApplicationPageByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetApplicationPageByIdQueryResponse>();
        response.Success = false;
        try
        {
            var result = await _pageRepository.GetPageForApplicationAsync(request.PageOrder, request.SectionId, request.FormVersionId);
            response.Value = result;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
        }

        return response;
    }
}
