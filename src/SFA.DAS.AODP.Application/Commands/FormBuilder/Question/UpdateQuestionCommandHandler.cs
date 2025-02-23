﻿using MediatR;
using SFA.DAS.AODP.Application;
using Newtonsoft.Json;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class UpdateQuestionCommandHandler(IQuestionRepository _questionRepository,
    IQuestionValidationRepository _questionValidationRepository,
    IQuestionOptionRepository _questionOptionRepository) : IRequestHandler<UpdateQuestionCommand, BaseMediatrResponse<EmptyResponse>>
{
    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();
        response.Success = false;

        try
        {
            if (!await _questionRepository.IsQuestionEditable(request.Id)) throw new RecordLockedException();

            var question = await _questionRepository.GetQuestionByIdAsync(request.Id);

            question.Title = request.Title;
            question.Hint = request.Hint;
            question.Required = request.Required;

            if (question.QuestionValidation == null)
            {
                question.QuestionValidation = new()
                {
                    QuestionId = request.Id,
                };
            }

            if (question.Type == QuestionType.Text.ToString())
            {
                question.QuestionValidation.MinLength = request.TextInput.MinLength;
                question.QuestionValidation.MaxLength = request.TextInput.MaxLength;

            }
            else if (question.Type == QuestionType.Radio.ToString() && request.RadioOptions != null)
            {
                question.QuestionOptions ??= new();

                var optionsToRemove = question.QuestionOptions.Where(q => !request.RadioOptions.Any(o => o.Id == q.Id)).ToList();
                await _questionOptionRepository.RemoveAsync(optionsToRemove);

                for (int i = 0; i < request.RadioOptions.Count; i++)
                {
                    if (request.RadioOptions[i].Id == default)
                    {
                        question.QuestionOptions.Add(new()
                        {
                            QuestionId = request.Id,
                            Order = i + 1,
                            Value = request.RadioOptions[i].Value
                        });
                    }
                    else
                    {
                        var option = question.QuestionOptions.First(q => q.Id == request.RadioOptions[i].Id);
                        option.Value = request.RadioOptions[i].Value;
                        option.Order = i + 1;
                    }

                }
                await _questionOptionRepository.UpsertAsync(question.QuestionOptions);


            }

            await _questionValidationRepository.UpsertAsync(question.QuestionValidation);
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
