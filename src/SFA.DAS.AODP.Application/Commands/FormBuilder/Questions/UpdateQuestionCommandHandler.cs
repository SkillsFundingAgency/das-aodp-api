using Markdig;
using Markdig.Syntax.Inlines;
using MediatR;
using Microsoft.Extensions.Options;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Models.Settings;
using System;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class UpdateQuestionCommandHandler(IQuestionRepository _questionRepository,
    IQuestionValidationRepository _questionValidationRepository,
    IQuestionOptionRepository _questionOptionRepository,
    FormBuilderSettings formBuilderSettings
   ) : IRequestHandler<UpdateQuestionCommand, BaseMediatrResponse<EmptyResponse>>
{
    public async Task<BaseMediatrResponse<EmptyResponse>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseMediatrResponse<EmptyResponse>();
        response.Success = false;

        try
        {
            if (!await _questionRepository.IsQuestionEditableAsync(request.Id)) throw new RecordLockedException();

            var question = await _questionRepository.GetQuestionByIdAsync(request.Id);
            ValidateQuestion(request, question);

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
            response.InnerException = ex;
            response.ErrorMessage = ex.Message;
        }

        return response;
    }

    private void ValidateQuestion(UpdateQuestionCommand request, Data.Entities.FormBuilder.Question question)
    {
        List<Exception> exceptions = new List<Exception>();
        if (question.Type == QuestionType.Radio.ToString()) { }
        if (question.Type == QuestionType.File.ToString())
        {
            if (request.FileUpload.NumberOfFiles > formBuilderSettings.MaxUploadNumberOfFiles)
            {
                exceptions.Add(new Exception(message:$"Max number of files allowed is {formBuilderSettings.MaxUploadNumberOfFiles}"));
            }
        }
        if (question.Type == QuestionType.Text.ToString())
        {
            exceptions.Concat(ValidateText(request, exceptions));
        }
        if (question.Type == QuestionType.TextArea.ToString())
        {
            exceptions.Concat(ValidateText(request, exceptions));
        }
        if (question.Type == QuestionType.Number.ToString())
        {
            if (request.NumberInput.LessThanOrEqualTo >= request.NumberInput.GreaterThanOrEqualTo)
            {
                exceptions.Add(new Exception(message:$"The number provided cannot be greater than or equal to {request.NumberInput.GreaterThanOrEqualTo}"));
            }
            if (request.NumberInput.GreaterThanOrEqualTo <= request.NumberInput.LessThanOrEqualTo)
            {
                exceptions.Add(new Exception(message:$"The number provided cannot be less than or equal to {request.NumberInput.LessThanOrEqualTo}"));
            }
            if (request.NumberInput.NotEqualTo <= request.NumberInput.LessThanOrEqualTo)
            {
                exceptions.Add(new Exception(message:$"The not allowed value/s provided cannot be less than or equal to {request.NumberInput.LessThanOrEqualTo}"));
            }
            if (request.NumberInput.NotEqualTo >= request.NumberInput.GreaterThanOrEqualTo)
            {
                exceptions.Add(new Exception(message:$"The not allowed value/s provided cannot be greater than or equal to {request.NumberInput.GreaterThanOrEqualTo}"));
            }
        }
        if (question.Type == QuestionType.MultiChoice.ToString())
        {
            if (request.Checkbox.MinNumberOfOptions < 0)
            {
                exceptions.Add(new Exception(message:$"The minimum number of checkbox options must not be a negative number"));
            }
            if (request.Checkbox.MaxNumberOfOptions < 0)
            {
                exceptions.Add(new Exception(message:$"The maximum number of checkbox options must not be a negative number"));
            }
            if (request.Checkbox.MinNumberOfOptions > request.Checkbox.MaxNumberOfOptions)
            {
                exceptions.Add(new Exception(message:$"The minimum number of checkbox options must be less than {request.Checkbox.MaxNumberOfOptions}"));
            }
            if (request.Checkbox.MaxNumberOfOptions < request.Checkbox.MinNumberOfOptions)
            {
                exceptions.Add(new Exception(message:$"The maximum number of checkbox options must be greater than {request.Checkbox.MinNumberOfOptions}"));
            }
            if (request.Options.Count < request.Checkbox.MinNumberOfOptions)
            {
                exceptions.Add(new Exception(message:$"The number of checkbox options cannot be less than {request.Checkbox.MinNumberOfOptions}"));
            }
            if (request.Options.Count > request.Checkbox.MaxNumberOfOptions)
            {
                exceptions.Add(new Exception(message:$"The number of checkbox options cannot be greater than {request.Checkbox.MaxNumberOfOptions}"));
            }
        }
        if (question.Type == QuestionType.Date.ToString())
        {
            if (request.DateInput.LessThanOrEqualTo >= request.DateInput.GreaterThanOrEqualTo)
            {
                exceptions.Add(new Exception(message:$"The date provided must be earlier than {request.DateInput.GreaterThanOrEqualTo}"));
            }
            if (request.DateInput.GreaterThanOrEqualTo <= request.DateInput.LessThanOrEqualTo)
            {
                exceptions.Add(new Exception(message:$"The date provided must be later than {request.DateInput.LessThanOrEqualTo}"));
            }
        }
        if (question.Type == null)
        {
            exceptions.Add(new Exception(message: $"The type provided must be valid."));
        }
        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
        }
    }

    private static List<Exception> ValidateText(UpdateQuestionCommand request, List<Exception> exceptions)
    {
        if (request.TextInput.MinLength > request.TextInput.MaxLength)
        {
            exceptions.Add(new Exception(message:$"The minimum length cannot be greater than {request.TextInput.MaxLength}"));
        }
        if (request.TextInput.MaxLength < request.TextInput.MinLength)
        {
            exceptions.Add(new Exception(message:$"The maximum length cannot be less than {request.TextInput.MinLength}"));
        }
        return exceptions;
    }
}
