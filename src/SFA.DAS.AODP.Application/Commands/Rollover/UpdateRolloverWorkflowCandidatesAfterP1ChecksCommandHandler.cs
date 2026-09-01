using MediatR;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Commands.Rollover;

public class UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandler : IRequestHandler<UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IRolloverRepository _rolloverRepository;

    public UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandler(IRolloverRepository rolloverRepository)
    {
        _rolloverRepository = rolloverRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();
        try
        {
            var candidates = await _rolloverRepository.GetAllRolloverWorkflowCandidatesAsync(cancellationToken);

            var p1CheckRequests = candidates
                .Select(candidate => new RolloverCandidateP1CheckRequest(
                    candidate.RolloverCandidatesId,
                    candidate.RolloverWorkflowRun.FundingEndDateEligibilityThreshold,
                    candidate.RolloverWorkflowRun.OperationalEndDateEligibilityThreshold,
                    candidate.RolloverWorkflowRun.MaximumApprovalFundingEndDate))
                .ToList();

            var candidatesWithP1Checks =
                await _rolloverRepository.GetRolloverCandidatesWithP1ChecksAsync(
                    p1CheckRequests,
                    cancellationToken);

            var checksByRolloverCandidateId = candidatesWithP1Checks
                .ToDictionary(x => x.Candidate.Id, x => x.P1Checks);
            var hasUpdates = false;

            foreach (var candidate in candidates)
            {
                if (!checksByRolloverCandidateId.TryGetValue(
                        candidate.RolloverCandidatesId,
                        out var check))
                {
                    continue;
                }

                candidate.ProcessP1Checks(check);
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                await _rolloverRepository.SaveChangesAsync(cancellationToken);
            }

            response.Success = true;
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
