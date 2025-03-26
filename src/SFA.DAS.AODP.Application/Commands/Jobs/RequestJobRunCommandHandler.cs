using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class RequestJobRunCommandHandler : IRequestHandler<RequestJobRunCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly ILogger<RequestJobRunCommandHandler> _logger;
        private readonly IJobRunsRepository _repository;

        public RequestJobRunCommandHandler(ILogger<RequestJobRunCommandHandler> logger, IJobRunsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(RequestJobRunCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();
            response.Success = false;
            try
            {
                var result = await _repository.RequestJobRun(request.JobName, request.UserName);               
                response.Success = result;
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
