using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, BaseMediatrResponse<EmptyResponse>>
    {
        private readonly ILogger<UpdateJobCommandHandler> _logger;
        private readonly IJobsRepository _repository;

        public UpdateJobCommandHandler(ILogger<UpdateJobCommandHandler> logger, IJobsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<EmptyResponse>();
            response.Success = false;
            try
            {
                var result = await _repository.UpdateJob(request.JobId, request.JobEnabled);
                response.Success = result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to update job");
                response.ErrorMessage = ex.Message;
            }

            return response;
        }
    }
}
