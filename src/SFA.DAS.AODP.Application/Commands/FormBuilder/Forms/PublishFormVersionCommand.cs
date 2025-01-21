using MediatR;
using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class PublishFormVersionCommand : IRequest<PublishFormVersionCommandResponse>
{
    public Guid FormVersionId { get; set; }
}
