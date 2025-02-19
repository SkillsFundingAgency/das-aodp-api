using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobsQueryQueryHandler : IRequestHandler<GetJobsQuery, BaseMediatrResponse<GetJobsQueryResponse>>
    {
        private readonly ILogger<GetJobsQueryQueryHandler> _logger;
        private readonly IJobsRepository _repository;

        public GetJobsQueryQueryHandler(ILogger<GetJobsQueryQueryHandler> logger, IJobsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetJobsQueryResponse>> Handle(GetJobsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetJobsQueryResponse>();
            response.Success = false;
            try
            {
                List<Job> result = await _repository.GetJobsAsync();
                response.Value = result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to get jobs from repository");
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
