using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public record CreateDraftFormVersionCommand(Guid FormId) : IRequest<CreateDraftFormVersionCommandResponse>;
