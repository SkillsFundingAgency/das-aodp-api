using MediatR;
using SFA.DAS.Aodp.Application.Application.Validation;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Application.Message;

[ExcludeFromCodeCoverage]
public class CreateApplicationMessageCommand : IRequest<BaseMediatrResponse<CreateApplicationMessageCommandResponse>>
{
    public Guid ApplicationId { get; set; }

    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string MessageText { get; set; }

    [MessageType]
    public string MessageType { get; set; }

    [UserType]
    public string UserType { get; set; }

    [AllowedCharacters(TextCharacterProfile.PersonName)]
    public string SentByName { get; set; }

    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string SentByEmail { get; set; }
}
