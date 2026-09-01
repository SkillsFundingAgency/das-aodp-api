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
                if (request.RolloverCandidateIds == null || !(request.RolloverCandidateIds.Count > 0))
                {
                    throw new InvalidOperationException("At least one rollover candidate must be provided.");
                }

                var p1CheckRequests = request.RolloverCandidateIds
                    .Select(id => new RolloverCandidateP1CheckRequest(
                        id,
                        request.FundingEndDateEligibilityThreshold,
                        request.OperationalEndDateEligibilityThreshold,
                        request.MaximumApprovalFundingEndDate))
                    .ToList();

                var candidatesWithP1Checks =
                    await _repository.GetRolloverCandidatesWithP1ChecksAsync(
                        p1CheckRequests,
                        cancellationToken);

                if (candidatesWithP1Checks.Count != request.RolloverCandidateIds.Count)
                {
                    throw new InvalidOperationException("One or more rollover candidate IDs are invalid or inactive.");
                }

                var now = DateTime.UtcNow;

                var workflowRun = RolloverWorkflowRun.Create(request.AcademicYear,
                    request.SelectionMethod, 
                    request.FundingEndDateEligibilityThreshold,
                    request.OperationalEndDateEligibilityThreshold, 
                    request.MaximumApprovalFundingEndDate, 
                    request.CreatedByUserName!,
                    now);

                var workflowCandidates = candidatesWithP1Checks
                   .Select(candidateData => RolloverWorkflowCandidate.Create(
                       workflowRun.Id,
                       candidateData.Candidate.Id,
                       candidateData.Candidate.QualificationVersionId,
                       candidateData.Candidate.FundingOfferId,
                       candidateData.Candidate.AcademicYear!,
                       candidateData.Candidate.RolloverRound,
                       candidateData.Candidate.PreviousFundingEndDate ?? now,
                       candidateData.Candidate.NewFundingEndDate,
                       now))
                   .ToList();

                var workflowCandidatesByRolloverCandidateId = workflowCandidates
                    .ToDictionary(x => x.RolloverCandidatesId);

                foreach (var candidateData in candidatesWithP1Checks)
                {
                    if (workflowCandidatesByRolloverCandidateId.TryGetValue(
                            candidateData.Candidate.Id,
                            out var workflowCandidate))
                    {
                        workflowCandidate.ProcessP1Checks(candidateData.P1Checks);
                    }
                }

                var workflowFundingOffers = request.FundingOfferIds
                    .Select(foId => RolloverWorkflowRunFundingOffer.Create(workflowRun.Id, foId))
                    .ToList();

                var workflowRunId = await _repository.CreateRolloverWorkflowAsync(
                    workflowRun,
                    workflowCandidates,
                    workflowFundingOffers,
                    cancellationToken);

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
