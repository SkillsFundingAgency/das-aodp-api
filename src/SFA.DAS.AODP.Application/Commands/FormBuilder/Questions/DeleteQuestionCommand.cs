using MediatR;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class DeleteQuestionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public DeleteQuestionCommand(Guid id) 
    {
        QuestionId = id;
    }
    public DeleteQuestionCommand() { }
    public Guid QuestionId { get; set; }

}