using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Questions;

public class DeleteQuestionCommand : IRequest<DeleteQuestionCommandResponse>
{
    public Guid QuestionId { get; set; }

}