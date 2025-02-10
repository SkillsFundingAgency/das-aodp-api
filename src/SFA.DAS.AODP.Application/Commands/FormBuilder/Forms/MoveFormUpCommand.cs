using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public record MoveFormUpCommand(Guid FormVersionId) : IRequest<BaseMediatrResponse<EmptyResponse>>;
