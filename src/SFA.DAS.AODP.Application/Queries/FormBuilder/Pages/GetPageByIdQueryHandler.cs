using AutoMapper;
using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Handlers.FormBuilder.Pages;

public class GetPageByIdQueryHandler(IPageRepository pageRepository, IMapper mapper) : IRequestHandler<GetPageByIdQuery, GetPageByIdQueryResponse>
{
    private readonly IPageRepository PageRepository = pageRepository;
    private readonly IMapper Mapper = mapper;

    public async Task<GetPageByIdQueryResponse> Handle(GetPageByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetPageByIdQueryResponse();
        response.Success = false;
        try
        {
            var page = await PageRepository.GetPageByIdAsync(request.PageId);

            response.Data = Mapper.Map<GetPageByIdQueryResponse.Page>(page);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.Success = false;
            response.InnerException = new NotFoundException(ex.Id);
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
