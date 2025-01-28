using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryHandler(IPageRepository pageRepository) : IRequestHandler<GetAllPagesQuery, GetAllPagesQueryResponse>
{
    private readonly IPageRepository PageRepository = pageRepository;
    

    public async Task<GetAllPagesQueryResponse> Handle(GetAllPagesQuery request, CancellationToken cancellationToken)
    {
        var response = new GetAllPagesQueryResponse();
        response.Success = false;
        try
        {
            var pages = await PageRepository.GetPagesForSectionAsync(request.SectionId);

            response.Data = [.. pages];
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}