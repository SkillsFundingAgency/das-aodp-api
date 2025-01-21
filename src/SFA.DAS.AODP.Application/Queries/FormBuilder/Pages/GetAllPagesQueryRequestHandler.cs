using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryRequestHandler : IRequestHandler<GetAllPagesQueryRequest, GetAllPagesQueryResponse>
{
    public GetAllPagesQueryRequestHandler()
    {
    }

    public async Task<GetAllPagesQueryResponse> Handle(GetAllPagesQueryRequest request, CancellationToken cancellationToken)
    {
        var response = new GetAllPagesQueryResponse(new());
        response.Success = false;
        try
        {
            // response.Data = await _sectionRepository.GetSectionsForFormAsync(request.FormId);
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}