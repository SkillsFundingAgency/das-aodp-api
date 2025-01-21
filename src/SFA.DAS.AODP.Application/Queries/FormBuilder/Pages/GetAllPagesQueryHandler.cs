using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryHandler : IRequestHandler<GetAllPagesQuery, GetAllPagesQueryResponse>
{
    public GetAllPagesQueryHandler()
    {
    }

    public async Task<GetAllPagesQueryResponse> Handle(GetAllPagesQuery request, CancellationToken cancellationToken)
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