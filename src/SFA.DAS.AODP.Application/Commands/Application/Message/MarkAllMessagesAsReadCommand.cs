using MediatR;

namespace SFA.DAS.AODP.Application.Commands.Application.Message;

public class MarkAllMessagesAsReadCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid ApplicationId { get; set; }
    public string UserType { get; set; }
}
