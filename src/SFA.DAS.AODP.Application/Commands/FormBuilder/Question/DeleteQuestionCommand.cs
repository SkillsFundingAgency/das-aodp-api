using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class DeleteQuestionCommand : IRequest<DeleteQuestionCommandResponse>
{
    public Guid QuestionId { get; set; }

}