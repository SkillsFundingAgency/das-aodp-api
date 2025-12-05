using MediatR;
namespace SFA.DAS.AODP.Application.Commands.Application;
public class WithdrawApplicationCommand : IRequest<BaseMediatrResponse<WithdrawApplicationCommandResponse>>
{
    public Guid ApplicationId { get; set; }
    public string WithdrawnBy { get; set; }
    public string WithdrawnByEmail { get; set; }
}