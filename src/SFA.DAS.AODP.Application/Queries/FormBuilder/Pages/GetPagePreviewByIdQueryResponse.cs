using SFA.DAS.AODP.Data.Entities.FormBuilder;
using static SFA.DAS.AODP.Application.Queries.FormBuilder.Questions.GetQuestionByIdQueryResponse;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetPagePreviewByIdQueryResponse
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }

    public List<Question> Questions { get; set; }

    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; }
        public bool Required { get; set; }
        public string? Hint { get; set; } = string.Empty;
        public int Order { get; set; }

        public TextInputOptions TextInput { get; set; } = new();
        public NumberInputOptions NumberInput { get; set; } = new();
        public CheckboxOptions Checkbox { get; set; } = new();
        public List<Option> Options { get; set; } = new();
        public DateInputOptions DateInput { get; set; } = new();


        public static implicit operator Question(SFA.DAS.AODP.Data.Entities.FormBuilder.Question entity)
        {
            var model = new Question()
            {
                Id = entity.Id,
                Title = entity.Title,
                Hint = entity.Hint,
                Order = entity.Order,
                Required = entity.Required,
                Type = entity.Type,
            };

            if (entity.Type == QuestionType.Text.ToString() && entity.QuestionValidation != null)
            {
                model.TextInput = new()
                {
                    MinLength = entity.QuestionValidation.MinLength,
                    MaxLength = entity.QuestionValidation.MaxLength,
                };
            }
            else if (entity.Type == QuestionType.Number.ToString())
            {
                model.NumberInput = new()
                {
                    GreaterThanOrEqualTo = entity.QuestionValidation.NumberGreaterThanOrEqualTo,
                    LessThanOrEqualTo = entity.QuestionValidation.NumberLessThanOrEqualTo,
                    NotEqualTo = entity.QuestionValidation.NumberNotEqualTo
                };
            }

            else if ((entity.Type == QuestionType.Radio.ToString() || entity.Type == QuestionType.MultiChoice.ToString()) && entity.QuestionOptions != null)
            {
                model.Options = new();
                foreach (var option in entity.QuestionOptions)
                {
                    model.Options.Add(new()
                    {
                        Id = option.Id,
                        Value = option.Value,
                        Order = option.Order,
                    });
                }


                if (model.Type == QuestionType.MultiChoice.ToString())
                {
                    model.Checkbox = new()
                    {
                        MaxNumberOfOptions = entity.QuestionValidation?.MaxNumberOfOptions ?? 0,
                        MinNumberOfOptions = entity.QuestionValidation?.MinNumberOfOptions ?? 0,
                    };
                }
            }
            else if (entity.Type == QuestionType.Date.ToString())
            {
                model.DateInput = new()
                {
                    GreaterThanOrEqualTo = entity.QuestionValidation?.DateGreaterThanOrEqualTo,
                    LessThanOrEqualTo = entity.QuestionValidation?.DateLessThanOrEqualTo,
                    MustBeInFuture = entity.QuestionValidation?.DateMustBeInFuture,
                    MustBeInPast = entity.QuestionValidation?.DateMustBeInPast,
                };
            }

            return model;
        }

    }

    public class TextInputOptions
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }

    }
    public class CheckboxOptions
    {
        public int? MinNumberOfOptions { get; set; }
        public int? MaxNumberOfOptions { get; set; }
    }

    public class NumberInputOptions
    {
        public int? GreaterThanOrEqualTo { get; set; }
        public int? LessThanOrEqualTo { get; set; }
        public int? NotEqualTo { get; set; }
    }


    public class Option
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }

    public class DateInputOptions
    {
        public DateOnly? GreaterThanOrEqualTo { get; set; }
        public DateOnly? LessThanOrEqualTo { get; set; }
        public bool? MustBeInFuture { get; set; }
        public bool? MustBeInPast { get; set; }
    }

    public static implicit operator GetPagePreviewByIdQueryResponse(Page entity)
    {
        return new()
        {
            Title = entity.Title,
            Order = entity.Order,
            Questions = entity.Questions != null ? [.. entity.Questions] : new(),
        };
    }
}