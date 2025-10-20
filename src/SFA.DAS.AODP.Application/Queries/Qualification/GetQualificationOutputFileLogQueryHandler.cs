using MediatR;
using SFA.DAS.AODP.Data.Repositories.Qualification;
namespace SFA.DAS.AODP.Application.Queries.Qualifications;


public class GetQualificationOutputFileLogQueryHandler : IRequestHandler<GetQualificationOutputFileLogQuery, BaseMediatrResponse<GetQualificationOutputFileLogResponse>>
{
    private readonly IQualificationOutputFileLogRepository _repository;

    public GetQualificationOutputFileLogQueryHandler(IQualificationOutputFileLogRepository repository )
    {
        _repository = repository;
    }

    public async Task<BaseMediatrResponse<GetQualificationOutputFileLogResponse>> Handle(GetQualificationOutputFileLogQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<GetQualificationOutputFileLogResponse>();

        try
        {
            var logs = await _repository.ListAsync();
            if (logs == null)
            {
                response.Success = false;
                response.ErrorMessage = "No logs found.";
            }
            else
            {
                response.Value = new GetQualificationOutputFileLogResponse
                {
                    OutputFileLogs = logs
                };
                response.Success = true;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}


