using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public record MoveFormDownCommand(Guid FormId) : IRequest<BaseMediatrResponse<EmptyResponse>>;
