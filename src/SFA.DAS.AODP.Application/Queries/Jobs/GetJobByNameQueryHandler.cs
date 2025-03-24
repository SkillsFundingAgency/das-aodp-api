using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobByNameQueryHandler : IRequestHandler<GetJobByNameQuery, BaseMediatrResponse<GetJobByNameQueryResponse>>
    {
        private readonly ILogger<GetJobByNameQueryHandler> _logger;
        private readonly IJobsRepository _repository;

        public GetJobByNameQueryHandler(ILogger<GetJobByNameQueryHandler> logger, IJobsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetJobByNameQueryResponse>> Handle(GetJobByNameQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetJobByNameQueryResponse>();
            response.Success = false;
            try
            {
                Job result = await _repository.GetJobByNameAsync(request.Name);
                response.Value = result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while trying to get job by name {request.Name} from repository");
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
