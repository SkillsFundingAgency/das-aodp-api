using MediatR;
using SFA.DAS.AODP.Application;
namespace SFA.DAS.AODP.Application.Commands.Application;
public class SubmitApplicationCommand : IRequest<BaseMediatrResponse<SubmitApplicationCommandResponse>>
{
    public Guid ApplicationId { get; set; }
    public string SubmittedBy { get; set; }
    public string SubmittedByEmail { get; set; }
}