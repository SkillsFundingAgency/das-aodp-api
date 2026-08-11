using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Application.Review;
[ExcludeFromCodeCoverage]
public class SaveSurveyCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string Page { get; set; }

    public int SatisfactionScore { get; set; }

    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string Comments { get; set; }
}
