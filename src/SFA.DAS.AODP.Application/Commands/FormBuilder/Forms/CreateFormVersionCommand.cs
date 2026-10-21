using MediatR;
using SFA.DAS.AODP.Application.Validators.Attributes;
using SFA.DAS.AODP.Application.Validators.Patterns;
using System.ComponentModel.DataAnnotations;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class CreateFormVersionCommand : IRequest<BaseMediatrResponse<CreateFormVersionCommandResponse>>
{
    [Required, MaxLength(200), AllowedCharacters(TextCharacterProfile.Title)]
    public string Title { get; set; }

    [MaxLength(4000), AllowedCharacters(TextCharacterProfile.FreeText)]
    public string Description { get; set; }
}
