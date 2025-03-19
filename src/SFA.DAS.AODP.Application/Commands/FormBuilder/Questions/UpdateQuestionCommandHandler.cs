using Markdig;
using MediatR;
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
            if (!await _questionRepository.IsQuestionEditableAsync(request.Id)) throw new RecordLockedException();

            var question = await _questionRepository.GetQuestionByIdAsync(request.Id);

            question.Title = request.Title;
            question.Hint = request.Hint;
            question.Required = request.Required;
            question.Helper = request.Helper;

            if (!string.IsNullOrEmpty(request.Helper))
            {
                question.HelperHTML = HTMLGenerator.FromMarkdown(request.Helper);
            }

            if (question.QuestionValidation == null)
            {
                question.QuestionValidation = new()
                {
                    QuestionId = request.Id,
                };
            }

            if (question.Type == QuestionType.Text.ToString() || question.Type == QuestionType.TextArea.ToString())
            {
                question.QuestionValidation.MinLength = request.TextInput.MinLength;
                question.QuestionValidation.MaxLength = request.TextInput.MaxLength;

            }
            else if (question.Type == QuestionType.Number.ToString())
            {
                question.QuestionValidation.NumberGreaterThanOrEqualTo = request.NumberInput.GreaterThanOrEqualTo;
                question.QuestionValidation.NumberLessThanOrEqualTo = request.NumberInput.LessThanOrEqualTo;
                question.QuestionValidation.NumberNotEqualTo = request.NumberInput.NotEqualTo;
            }
            else if (question.Type == QuestionType.Date.ToString())
            {
                question.QuestionValidation.DateGreaterThanOrEqualTo = request.DateInput.GreaterThanOrEqualTo;
                question.QuestionValidation.DateLessThanOrEqualTo = request.DateInput.LessThanOrEqualTo;
                question.QuestionValidation.DateMustBeInFuture = request.DateInput.MustBeInFuture;
                question.QuestionValidation.DateMustBeInPast = request.DateInput.MustBeInPast;
            }
            else if (question.Type == QuestionType.File.ToString())
            {
                question.QuestionValidation.FileMaxSize = request.FileUpload.MaxSize;
                question.QuestionValidation.FileNamePrefix = request.FileUpload.FileNamePrefix;
                question.QuestionValidation.NumberOfFiles = request.FileUpload.NumberOfFiles;
            }
            else if ((question.Type == QuestionType.MultiChoice.ToString() || question.Type == QuestionType.Radio.ToString()))
            {
                question.QuestionOptions ??= new();

                request.Options ??= [];

                var optionsToRemove = question.QuestionOptions.Where(q => !request.Options.Any(o => o.Id == q.Id)).ToList();
                await _questionOptionRepository.RemoveAsync(optionsToRemove);

                for (int i = 0; i < request.Options.Count; i++)
                {
                    if (request.Options[i].Id == default)
                    {
                        question.QuestionOptions.Add(new()
                        {
                            QuestionId = request.Id,
                            Order = i + 1,
                            Value = request.Options[i].Value
                        });
                    }
                    else
                    {
                        var option = question.QuestionOptions.First(q => q.Id == request.Options[i].Id);
                        option.Value = request.Options[i].Value;
                        option.Order = i + 1;
                    }

                }
                await _questionOptionRepository.UpsertAsync(question.QuestionOptions);

                if (question.Type == QuestionType.MultiChoice.ToString())
                {
                    question.QuestionValidation.MinNumberOfOptions = request.Checkbox.MinNumberOfOptions;
                    question.QuestionValidation.MaxNumberOfOptions = request.Checkbox.MaxNumberOfOptions;
                }
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
