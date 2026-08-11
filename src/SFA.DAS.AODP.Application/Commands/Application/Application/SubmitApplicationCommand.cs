using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;
namespace SFA.DAS.AODP.Application.Commands.Application;

[ExcludeFromCodeCoverage]
public class SubmitApplicationCommand : IRequest<BaseMediatrResponse<SubmitApplicationCommandResponse>>
{
    public Guid ApplicationId { get; set; }

    [AllowedCharacters(TextCharacterProfile.PersonName)]
    public string SubmittedBy { get; set; }

    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string SubmittedByEmail { get; set; }
}