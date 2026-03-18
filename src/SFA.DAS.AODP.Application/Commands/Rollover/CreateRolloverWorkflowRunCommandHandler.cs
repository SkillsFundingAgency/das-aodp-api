using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Commands.Rollover
{
    public class CreateRolloverWorkflowRunCommandHandler
    : IRequestHandler<CreateRolloverWorkflowRunCommand, BaseMediatrResponse<CreateRolloverWorkflowRunCommandResponse>>
    {
        private readonly IRolloverRepository _rolloverRepository;

        public CreateRolloverWorkflowRunCommandHandler(IRolloverRepository rolloverRepository)
        {
            _rolloverRepository = rolloverRepository;
        }

        public async Task<BaseMediatrResponse<CreateRolloverWorkflowRunCommandResponse>> Handle(CreateRolloverWorkflowRunCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseMediatrResponse<CreateRolloverWorkflowRunCommandResponse>();

            try
            {
                if (request.RolloverCandidateIds == null || !request.RolloverCandidateIds.Any())
                {
                    throw new InvalidOperationException("At least one rollover candidate must be provided.");
                }

                var repoRequest = RolloverWorkflowRun.Create(request.AcademicYear, Data.Entities.Rollover.Enums.SelectionMethod.FileUpload, request.FundingEndDateEligibilityThreshold,
                    request.OperationalEndDateEligibilityThreshold, request.MaximumApprovalFundingEndDate, request.CreatedByUserName, DateTime.Now);

                var workflowRunId = await _rolloverRepository.CreateRolloverWorkflowRunAsync(repoRequest, request.RolloverCandidateIds, cancellationToken);

                response.Value = new CreateRolloverWorkflowRunCommandResponse
                {
                    RolloverWorkflowRunId = workflowRunId
                };

                response.Success = true;
            }
            catch (RecordLockedException)
            {
                response.Success = false;
                response.InnerException = new LockedRecordException();
            }
            catch (NoForeignKeyException ex)
            {
                response.Success = false;
                response.InnerException = new DependantNotFoundException(ex.ForeignKey);
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
}
