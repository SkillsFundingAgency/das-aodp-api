using MediatR;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

namespace SFA.DAS.AODP.Application.Handlers.FormBuilder.Pages;

public class GetPageByIdQueryHandler : IRequestHandler<GetPageByIdQuery, GetPageByIdQueryResponse>
{
    public GetPageByIdQueryHandler()
    {
    }

    public async Task<GetPageByIdQueryResponse> Handle(GetPageByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetPageByIdQueryResponse(new());
        response.Success = false;
        try
        {
            //response.Data = _pageRepository.GetById(request.Id);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
