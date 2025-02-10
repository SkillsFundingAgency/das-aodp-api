using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public record UnpublishFormVersionCommand(Guid FormVersionId) : IRequest<BaseMediatrResponse<EmptyResponse>>;
