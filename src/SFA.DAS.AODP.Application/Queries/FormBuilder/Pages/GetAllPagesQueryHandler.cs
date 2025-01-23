using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryHandler(IPageRepository pageRepository, IMapper mapper) : IRequestHandler<GetAllPagesQuery, GetAllPagesQueryResponse>
{
    private readonly IPageRepository PageRepository = pageRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<GetAllPagesQueryResponse> Handle(GetAllPagesQuery request, CancellationToken cancellationToken)
    {
        var response = new GetAllPagesQueryResponse();
        response.Success = false;
        try
        {
            var pages = await PageRepository.GetPagesForSectionAsync(request.SectionId);

            response.Data = Mapper.Map<List<GetAllPagesQueryResponse.Page>>(pages);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}