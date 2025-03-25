using MediatR;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UpdateJobCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid JobId { get; set; }  
    public bool JobEnabled { get; set; }
}
