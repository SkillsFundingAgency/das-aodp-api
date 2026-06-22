using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Application.Message;

[ExcludeFromCodeCoverage]
public class MarkAllMessagesAsReadCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid ApplicationId { get; set; }
    [UserType]
    public string UserType { get; set; }
}
