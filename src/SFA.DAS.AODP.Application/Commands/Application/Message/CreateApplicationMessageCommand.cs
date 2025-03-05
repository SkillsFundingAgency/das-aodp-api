using MediatR;

namespace SFA.DAS.AODP.Application.Commands.Application.Message;

public class CreateApplicationMessageCommand : IRequest<BaseMediatrResponse<CreateApplicationMessageCommandResponse>>
{
    public Guid ApplicationId { get; set; }
    public string MessageText { get; set; }
    public string MessageType { get; set; }
    public bool SharedWithDfe { get; set; }
    public bool SharedWithOfqual { get; set; }
    public bool SharedWithSkillsEngland { get; set; }
    public bool SharedWithAwardingOrganisation { get; set; }
    public string SentByName { get; set; }
    public string SentByEmail { get; set; } // this to get from the cookie?
}
