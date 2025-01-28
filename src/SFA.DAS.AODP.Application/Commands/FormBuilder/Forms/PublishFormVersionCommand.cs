using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public record PublishFormVersionCommand(Guid FormVersionId) : IRequest<PublishFormVersionCommandResponse>;
