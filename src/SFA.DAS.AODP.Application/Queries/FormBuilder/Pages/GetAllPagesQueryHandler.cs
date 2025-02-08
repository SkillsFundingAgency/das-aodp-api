using MediatR;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryHandler(IPageRepository pageRepository) : IRequestHandler<GetAllPagesQuery, BaseMediatrResponse< GetAllPagesQueryResponse>>
{
    private readonly IPageRepository PageRepository = pageRepository;
    

    public async Task<BaseMediatrResponse<GetAllPagesQueryResponse>> Handle(GetAllPagesQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetAllPagesQueryResponse>();
        response.Success = false;
        try
        {
            var pages = await PageRepository.GetPagesForSectionAsync(request.SectionId);

            response.Value.Data = [.. pages];
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}