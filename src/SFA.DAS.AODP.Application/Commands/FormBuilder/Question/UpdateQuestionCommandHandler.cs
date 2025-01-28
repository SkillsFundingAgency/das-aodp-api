using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Questions;

public class UpdateQuestionCommandHandler(IQuestionRepository _questionRepository) : IRequestHandler<UpdateQuestionCommand, UpdateQuestionCommandResponse>
{
    public async Task<UpdateQuestionCommandResponse> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateQuestionCommandResponse();
        response.Success = false;

        try
        {
            var question = await _questionRepository.GetQuestionByIdAsync(request.Id);
            question.Title = request.Title;
            question.Hint = request.Hint;
            question.Required = request.Required;

            if (question.Type == QuestionType.Text.ToString())
            {
                question.TextValidator = JsonConvert.SerializeObject(request.TextInput);
            }
            else if (question.Type == QuestionType.Radio.ToString())
            {
                question.RadioValidator = JsonConvert.SerializeObject(request.RadioOptions);
            }

            await _questionRepository.Update(question);
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
            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}
