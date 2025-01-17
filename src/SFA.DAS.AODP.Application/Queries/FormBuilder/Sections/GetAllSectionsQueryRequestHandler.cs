using MediatR;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQueryRequestHandler : IRequestHandler<GetAllSectionsQueryRequest, GetAllSectionsQueryResponse>
{
    public GetAllSectionsQueryRequestHandler()
    {
    }

    public async Task<GetAllSectionsQueryResponse> Handle(GetAllSectionsQueryRequest request, CancellationToken cancellationToken)
    {
        var response = new GetAllSectionsQueryResponse();
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