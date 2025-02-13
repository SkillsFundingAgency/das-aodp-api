using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public record MoveFormUpCommand(Guid FormId) : IRequest<BaseMediatrResponse<EmptyResponse>>;
