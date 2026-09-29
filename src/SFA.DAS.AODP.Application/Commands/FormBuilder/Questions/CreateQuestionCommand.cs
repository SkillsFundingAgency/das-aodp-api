using MediatR;
using SFA.DAS.Aodp.Application.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

[ExcludeFromCodeCoverage]
public class CreateQuestionCommand : IRequest<BaseMediatrResponse<CreateQuestionCommandResponse>>
{
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
    public Guid PageId { get; set; }

    [AllowedCharacters(TextCharacterProfile.Title)]
    public string Title { get; set; }
    public bool Required { get; set; }

    [QuestionType]
    public string Type { get; set; }
}
