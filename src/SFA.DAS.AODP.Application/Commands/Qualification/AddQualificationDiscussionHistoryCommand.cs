using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Qualification;

[ExcludeFromCodeCoverage]
public class AddQualificationDiscussionHistoryCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    [QualificationNumber]
    public string QualificationReference { get; set; } = string.Empty;

    [AllowedCharacters(TextCharacterProfile.PersonName)]
    public string? UserDisplayName { get; set; }

    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string? Notes { get; set; }
}
