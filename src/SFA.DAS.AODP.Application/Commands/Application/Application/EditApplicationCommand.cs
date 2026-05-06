using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;
namespace SFA.DAS.AODP.Application.Commands.Application.Application;

[ExcludeFromCodeCoverage]
public class EditApplicationCommand : IRequest<BaseMediatrResponse<EditApplicationCommandResponse>>
{
    [QualificationNumber]
    public string? QualificationNumber { get; set; }
    [AllowedCharacters(TextCharacterProfile.Title)]
    public string Title { get; set; }
    [AllowedCharacters(TextCharacterProfile.PersonName)]
    public string Owner { get; set; }
    public Guid ApplicationId { get; set; }
}
