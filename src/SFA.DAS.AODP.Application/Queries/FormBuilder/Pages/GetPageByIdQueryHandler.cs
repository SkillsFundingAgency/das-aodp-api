using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Handlers.FormBuilder.Pages;

public class GetPageByIdQueryHandler(IPageRepository pageRepository, IMapper mapper) : IRequestHandler<GetPageByIdQuery, GetPageByIdQueryResponse>
{
    private readonly IPageRepository PageRepository = pageRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<GetPageByIdQueryResponse> Handle(GetPageByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetPageByIdQueryResponse(new());
        response.Success = false;
        try
        {
            var page = await PageRepository.GetPageByIdAsync(request.PageId);

            if (page is null)
            {
                response.Success = false;
                response.ErrorMessage = $"Page with id '{request.PageId}' could not be found.";
                return response;
            }

            response.Data = Mapper.Map<GetPageByIdQueryResponse.Page>(page);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
