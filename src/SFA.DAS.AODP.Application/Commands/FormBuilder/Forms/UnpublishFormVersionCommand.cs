using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public record UnpublishFormVersionCommand(Guid FormVersionId) : IRequest<UnpublishFormVersionCommandResponse>;
