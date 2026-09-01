using MediatR;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.Commands.Rollover;

public class RemovePreviousWorkflowCandidatesCommandHandler : IRequestHandler<RemovePreviousWorkflowCandidatesCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IRolloverRepository _rolloverRepository;

    public RemovePreviousWorkflowCandidatesCommandHandler(IRolloverRepository rolloverRepository)
    {
        _rolloverRepository = rolloverRepository;
    }

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(RemovePreviousWorkflowCandidatesCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();
        try
        {
            await _rolloverRepository.DeleteAllWorkflowCandidatesAsync(cancellationToken);
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
