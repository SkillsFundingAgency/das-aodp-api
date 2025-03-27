using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobRunsQueryHandler : IRequestHandler<GetJobRunsQuery, BaseMediatrResponse<GetJobRunsQueryResponse>>
    {
        private readonly ILogger<GetJobRunsQueryHandler> _logger;
        private readonly IJobRunsRepository _repository;

        public GetJobRunsQueryHandler(ILogger<GetJobRunsQueryHandler> logger, IJobRunsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetJobRunsQueryResponse>> Handle(GetJobRunsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetJobRunsQueryResponse>();
            response.Success = false;
            try
            {
                List<JobRun> result = await _repository.GetJobRunsAsync(request.JobName);
                response.Value = result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to get job runs from repository");
                response.InnerException = ex;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
