using MediatR;

namespace SFA.DAS.AODP.Application.Commands.Rollover;

public record RemovePreviousWorkflowCandidatesCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
}
