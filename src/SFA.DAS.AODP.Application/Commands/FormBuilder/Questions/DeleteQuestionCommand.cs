using MediatR;
namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class DeleteQuestionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid QuestionId { get; set; }

}