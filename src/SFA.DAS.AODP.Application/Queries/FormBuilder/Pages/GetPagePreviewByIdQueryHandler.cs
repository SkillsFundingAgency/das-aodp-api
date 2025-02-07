using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetPagePreviewByIdQueryHandler : IRequestHandler<GetPagePreviewByIdQuery, GetPagePreviewByIdQueryResponse>
{
    private readonly IPageRepository _pageRepository;

    public GetPagePreviewByIdQueryHandler(IPageRepository pageRepository) => _pageRepository = pageRepository;

    public async Task<GetPagePreviewByIdQueryResponse> Handle(GetPagePreviewByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetPagePreviewByIdQueryResponse();
        response.Success = false;
        try
        {
            var page = await _pageRepository.GetPageByIdAsync(request.PageId);
            var result = await _pageRepository.GetPageForApplicationAsync(page.Order, request.SectionId);
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