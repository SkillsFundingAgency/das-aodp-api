using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

[ExcludeFromCodeCoverage]
public class CreateSectionCommand : IRequest<BaseMediatrResponse<CreateSectionCommandResponse>>
{

    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid Key { get; set; }
    public int Order { get; set; }
    [AllowedCharacters(TextCharacterProfile.Title)]
    public string Title { get; set; } = string.Empty;
    public int? NextSectionId { get; set; }
}
