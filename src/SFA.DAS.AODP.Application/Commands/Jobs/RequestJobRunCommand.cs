using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

[ExcludeFromCodeCoverage]
public class RequestJobRunCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string JobName { get; set; }
    [AllowedCharacters(TextCharacterProfile.PersonName)]
    public string UserName { get; set; }
}
