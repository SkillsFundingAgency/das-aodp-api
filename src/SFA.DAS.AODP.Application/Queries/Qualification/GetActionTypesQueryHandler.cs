using Azure;
using MediatR;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications;


public class GetActionTypesQueryHandler : IRequestHandler<GetActionTypesQuery, BaseMediatrResponse<GetActionTypesQueryResponse>>
{
    private readonly IChangedQualificationsRepository _repository;

    public GetActionTypesQueryHandler(IChangedQualificationsRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseMediatrResponse<GetActionTypesQueryResponse>> Handle(GetActionTypesQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetActionTypesQueryResponse>();
        try
        {
            var actionTypes = await _repository.GetActionTypes();
            if (actionTypes != null)
            {
                response.Value = new GetActionTypesQueryResponse()
                {
                    ActionTypes = actionTypes.Select(v => new GetActionTypesQueryResponse.ActionType
                    {
                        Id = v.Id,
                        Description = v.Description
                    }).ToList()
                };

                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = $"No ActionTypes ";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
            response.InnerException = ex;
        }
        return response;
    }
}


