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
        private readonly IRolloverRepository _repository;

        public CreateRolloverWorkflowRunCommandHandler(IRolloverRepository rolloverRepository)
        {
            _repository = rolloverRepository;
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

                var candidates = await _repository.GetRolloverCandidatesByIdsAsync(request.RolloverCandidateIds, cancellationToken);
                if (candidates.Count() != request.RolloverCandidateIds.Count())
                {
                    throw new InvalidOperationException("One or more rollover candidate IDs are invalid or inactive.");
                }

                var now = DateTime.UtcNow;

                var workflowrun = RolloverWorkflowRun.Create(request.AcademicYear, 
                    Data.Entities.Rollover.Enums.SelectionMethod.FileUpload, 
                    request.FundingEndDateEligibilityThreshold,
                    request.OperationalEndDateEligibilityThreshold, 
                    request.MaximumApprovalFundingEndDate, 
                    request.CreatedByUserName!,
                    now);
                var workflowRunId = await _repository.CreateRolloverWorkflowRunAsync(workflowrun, cancellationToken);

                var workflowCandidates = candidates
                   .Select(rc => RolloverWorkflowCandidate.Create(
                       workflowRunId,
                       rc.Id,
                       rc.QualificationVersionId,
                       rc.FundingOfferId,
                       rc.AcademicYear!,
                       rc.RolloverRound,
                       rc.PreviousFundingEndDate ?? now,
                       rc.NewFundingEndDate,
                       now));
                await _repository.CreateRolloverWorkflowCandidatesAsync(workflowCandidates, cancellationToken);

                var workflowFundingOffers = request.FundingOfferIds
                    .Select(foId => RolloverWorkflowRunFundingOffer.Create(workflowRunId, foId))
                    .ToList();
                await _repository.CreateRolloverWorkflowRunFundingOffersAsync(workflowFundingOffers, cancellationToken);

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
