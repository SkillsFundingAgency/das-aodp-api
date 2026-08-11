using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;
namespace SFA.DAS.AODP.Application.Commands.Application;

[ExcludeFromCodeCoverage]
public class WithdrawApplicationCommand : IRequest<BaseMediatrResponse<WithdrawApplicationCommandResponse>>
{
    public Guid ApplicationId { get; set; }

    [AllowedCharacters(TextCharacterProfile.PersonName)]
    public required string WithdrawnBy { get; set; }

    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public required string WithdrawnByEmail { get; set; }
}