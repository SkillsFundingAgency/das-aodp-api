using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder;

public class MoveQuestionDownCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid QuestionId { get; set; }
    public Guid PageId { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
}
