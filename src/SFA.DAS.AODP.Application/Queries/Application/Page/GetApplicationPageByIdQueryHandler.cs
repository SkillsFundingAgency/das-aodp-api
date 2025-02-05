using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

public class GetApplicationPageByIdQueryHandler : IRequestHandler<GetApplicationPageByIdQuery, GetApplicationPageByIdQueryResponse>
{
    private readonly IPageRepository _pageRepository;

    public GetApplicationPageByIdQueryHandler(IPageRepository pageRepository) => _pageRepository = pageRepository;

    public async Task<GetApplicationPageByIdQueryResponse> Handle(GetApplicationPageByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetApplicationPageByIdQueryResponse();
        response.Success = false;
        try
        {
            var result = await _pageRepository.GetPageForApplicationAsync(request.PageOrder, request.SectionId);
            response = result;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
