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
            var p1ValidationCheckData = await _rolloverRepository.GetRolloverWorkflowCandidatesP1ChecksAsync(cancellationToken);

            var candidates = await _rolloverRepository.GetAllRolloverWorkflowCandidatesAsync(cancellationToken);

            var candidatesById = candidates.ToDictionary(x => x.Id);

            var candidatesToUpdate = new List<RolloverWorkflowCandidate>();

            foreach (var check in p1ValidationCheckData)
            {
                if (!candidatesById.TryGetValue(
                        check.WorkflowCandidateId,
                        out var candidate))
                {
                    continue;
                }

                candidate.ProcessP1Checks(check);

                candidatesToUpdate.Add(candidate);
            }

            if (candidatesToUpdate.Any())
            {
                await _rolloverRepository
                    .UpdateRolloverWorkflowCandidatesAsync(
                        candidatesToUpdate,
                        cancellationToken);
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
