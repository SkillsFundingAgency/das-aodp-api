using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobRunByIdQueryHandler : IRequestHandler<GetJobRunByIdQuery, BaseMediatrResponse<GetJobRunByIdQueryResponse>>
    {
        private readonly ILogger<GetJobRunByIdQueryHandler> _logger;
        private readonly IJobRunsRepository _repository;

        public GetJobRunByIdQueryHandler(ILogger<GetJobRunByIdQueryHandler> logger, IJobRunsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<GetJobRunByIdQueryResponse>> Handle(GetJobRunByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<GetJobRunByIdQueryResponse>();
            response.Success = false;
            try
            {
                var result = await _repository.GetJobRunsById(request.Id);
                response.Value = result;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while trying to get job runs from repository with id {request.Id}");
                response.InnerException = ex;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
