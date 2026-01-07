using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Application.Application;

public class EditApplicationCommand : IRequest<BaseMediatrResponse<EditApplicationCommandResponse>>
{
    public string? QualificationNumber { get; set; }
    public required string Title { get; set; }
    public required string Owner { get; set; }
    public Guid ApplicationId { get; set; }
}
