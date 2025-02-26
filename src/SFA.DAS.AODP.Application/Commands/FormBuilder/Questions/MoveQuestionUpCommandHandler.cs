using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder;

public class MoveQuestionUpCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<MoveQuestionUpCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(MoveQuestionUpCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>()
        {
        };

        try
        {
            if (!await _questionRepository.IsQuestionEditable(request.QuestionId)) throw new RecordLockedException();

            var res = await _questionRepository.MoveQuestionOrderUp(request.QuestionId);
            response.Success = true;
        }
        catch (RecordNotFoundException ex)
        {
            response.Success = false;
            response.InnerException = new NotFoundException(ex.Id);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
