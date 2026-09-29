using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;

[ExcludeFromCodeCoverage]
public class UpdateSectionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid FormVersionId { get; set; }
    public Guid Id { get; set; }
    [AllowedCharacters(TextCharacterProfile.Title)]
    public string Title { get; set; }
}
