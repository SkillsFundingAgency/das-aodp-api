using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public record UnpublishFormVersionCommand(Guid FormVersionId) : IRequest<UnpublishFormVersionCommandResponse>;
