namespace SFA.DAS.AODP.Application.Commands.Application.Message;

public class CreateApplicationMessageCommandResponse
{
    public Guid Id { get; set; }
    public List<NotificationDefinition> Notifications { get; set; } = new();
}