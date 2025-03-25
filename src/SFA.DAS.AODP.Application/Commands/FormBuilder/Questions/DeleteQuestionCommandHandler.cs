using MediatR;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class DeleteQuestionCommandHandler(IQuestionRepository _questionRepository) : IRequestHandler<DeleteQuestionCommand, BaseMediatrResponse<EmptyResponse>>
{
    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();

        try
        {
            if (!await _questionRepository.IsQuestionEditableAsync(request.QuestionId)) throw new RecordLockedException();

            await _questionRepository.Archive(request.QuestionId);

            response.Success = true;
        }
        catch (RecordLockedException)
        {
            response.Success = false;
            response.InnerException = new LockedRecordException();
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
