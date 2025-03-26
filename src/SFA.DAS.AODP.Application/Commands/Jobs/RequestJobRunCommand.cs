using MediatR;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class RequestJobRunCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public string JobName { get; set; }
    public string UserName { get; set; }
}
