using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class CreateQuestionCommand : IRequest<BaseMediatrResponse<CreateQuestionCommandResponse>>
{
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
    public Guid PageId { get; set; }
    public string Title { get; set; }
    public bool Required { get; set; }
    public string Type { get; set; }
}
