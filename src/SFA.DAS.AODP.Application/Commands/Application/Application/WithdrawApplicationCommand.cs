using MediatR;
namespace SFA.DAS.AODP.Application.Commands.Application;
public class WithdrawApplicationCommand : IRequest<BaseMediatrResponse<WithdrawApplicationCommandResponse>>
{
    public Guid ApplicationId { get; set; }
    public required string WithdrawnBy { get; set; }
    public required string WithdrawnByEmail { get; set; }
}