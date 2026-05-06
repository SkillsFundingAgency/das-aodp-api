using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

[ExcludeFromCodeCoverage]
public class CreatePageCommand : IRequest<BaseMediatrResponse<CreatePageCommandResponse>>
{
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
    [AllowedCharacters(TextCharacterProfile.Title)]
    public string Title { get; set; }
}
