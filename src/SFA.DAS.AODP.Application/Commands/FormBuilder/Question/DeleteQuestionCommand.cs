using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class DeleteQuestionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid QuestionId { get; set; }

}