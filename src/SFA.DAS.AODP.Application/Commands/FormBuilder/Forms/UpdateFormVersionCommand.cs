using MediatR;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Validators.Attributes;
using SFA.DAS.AODP.Application.Validators.Patterns;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

public class UpdateFormVersionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid FormVersionId { get; set; }

    [Required, MaxLength(200), AllowedCharacters(TextCharacterProfile.Title)]
    public string Name { get; set; }

    [MaxLength(4000), AllowedCharacters(TextCharacterProfile.FreeText)]
    public string Description { get; set; }

}