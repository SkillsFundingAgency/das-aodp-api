using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Qualification;

[ExcludeFromCodeCoverage]
public class UpdateQualificationStatusCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    [QualificationNumber]
    public string QualificationReference { get; set; } = string.Empty;
    public Guid ProcessStatusId { get; set; }

    [AllowedCharacters(TextCharacterProfile.PersonName)]
    public string? UserDisplayName { get; set; }
    public int? Version { get; set; }

    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string? Notes { get; set; }
}
