using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

[ExcludeFromCodeCoverage]
public class UpdateFormVersionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid FormVersionId { get; set; }

    [AllowedCharacters(TextCharacterProfile.Title)]
    public string Name { get; set; }
    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string Description { get; set; }
    public int Order { get; set; }

}