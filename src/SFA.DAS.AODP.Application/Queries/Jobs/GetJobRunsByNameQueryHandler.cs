using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobRunsByNameQueryHandler : IRequestHandler<GetJobRunsByNameQuery, BaseMediatrResponse<GetJobRunsByNameQueryResponse>>
    {
        private readonly ILogger<GetJobRunsByNameQueryHandler> _logger;
        private readonly IJobRunsRepository _repository;

        public GetJobRunsByNameQueryHandler(ILogger<GetJobRunsByNameQueryHandler> logger, IJobRunsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetJobRunsByNameQueryResponse>> Handle(GetJobRunsByNameQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetJobRunsByNameQueryResponse>();
            response.Success = false;
            try
            {
                List<JobRun> result = await _repository.GetJobRunsByNameAsync(request.JobName);
                response.Value = result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to get job runs by name from repository");
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
