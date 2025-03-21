using Azure;
using MediatR;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.Queries.Qualifications;


public class GetProcessingStatusesQueryHandler : IRequestHandler<GetProcessingStatusesQuery, BaseMediatrResponse<GetProcessingStatusesQueryResponse>>
{
    private readonly IQualificationsRepository _repository;

    public GetProcessingStatusesQueryHandler(IQualificationsRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseMediatrResponse<GetProcessingStatusesQueryResponse>> Handle(GetProcessingStatusesQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetProcessingStatusesQueryResponse>();
        try
        {
            var processStatuses = await _repository.GetProcessingStatuses();
            if (processStatuses != null)
            {
                response.Value.ProcessStatuses = [..processStatuses];

                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.ErrorMessage = $"No process statuses ";
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


