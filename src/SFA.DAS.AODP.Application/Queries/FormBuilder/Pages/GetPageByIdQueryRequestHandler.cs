using MediatR;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

namespace SFA.DAS.AODP.Application.Handlers.FormBuilder.Pages;

public class GetPageByIdQueryRequestHandler : IRequestHandler<GetPageByIdQueryRequest, GetPageByIdQueryResponse>
{
    public GetPageByIdQueryRequestHandler()
    {
    }

    public async Task<GetPageByIdQueryResponse> Handle(GetPageByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var response = new GetPageByIdQueryResponse();
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
