using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder;

public class MoveQuestionDownCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<MoveQuestionDownCommand, BaseMediatrResponse<EmptyResponse>>
{
    private readonly IQuestionRepository _questionRepository = questionRepository;

    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(MoveQuestionDownCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>()
        {
        };

        try
        {
            if (!await _questionRepository.IsQuestionEditableAsync(request.QuestionId)) throw new RecordLockedException();

            var res = await _questionRepository.MoveQuestionOrderDown(request.QuestionId);
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
