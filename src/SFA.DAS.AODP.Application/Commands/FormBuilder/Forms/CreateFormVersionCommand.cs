using MediatR;
using SFA.DAS.Aodp.Application.Validation;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormVersionCommand : IRequest<BaseMediatrResponse<CreateFormVersionCommandResponse>>
{
    [AllowedCharacters(TextCharacterProfile.Title)]
    public string Title { get; set; }

    [AllowedCharacters(TextCharacterProfile.FreeText)]
    public string Description { get; set; }
    public int Order { get; set; }
}
