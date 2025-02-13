using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UpdateFormVersionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid FormVersionId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

}