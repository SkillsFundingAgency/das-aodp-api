using MediatR;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetAllFormVersionsQueryHandler : IRequestHandler<GetAllFormVersionsQuery, GetAllFormVersionsQueryResponse>
{
    private readonly IFormVersionRepository _formRepository;


    public GetAllFormVersionsQueryHandler(IFormVersionRepository formRepository)
    {
        _formRepository = formRepository;

    }

    public async Task<GetAllFormVersionsQueryResponse> Handle(GetAllFormVersionsQuery request, CancellationToken cancellationToken)
    {
        var queryResponse = new GetAllFormVersionsQueryResponse()
        {
            Success = false
        };
        try
        {
            var data = await _formRepository.GetLatestFormVersions();

            queryResponse.Data = [.. data];

            queryResponse.Success = true;
        }
        catch (Exception ex)
        {
            queryResponse.ErrorMessage = ex.Message;
        }

        return queryResponse;
    }
}