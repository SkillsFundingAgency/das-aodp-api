using Newtonsoft.Json;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Routes;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;

public class GetQuestionByIdQueryResponse()
{

    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public string Title { get; set; }
    public Guid Key { get; set; }
    public string Hint { get; set; }
    public int Order { get; set; }
    public bool Required { get; set; }
    public string Type { get; set; }

    public TextInputOptions TextInput { get; set; } = new();
    public NumberInputOptions NumberInput { get; set; } = new();
    public CheckboxOptions Checkbox { get; set; } = new();
    public DateInputOptions DateInput { get; set; } = new();
    public List<RadioOptionItem> RadioOptions { get; set; } = new();
    public List<RouteInformation> Routes { get; set; } = new();


    public List<Option> Options { get; set; } = new();

    public bool Editable { get; set; }
    public class TextInputOptions
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }
    public class Option
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }

    public class RadioOptionItem
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }

    public class RouteInformation
    {
        public Page? NextPage { get; set; }
        public Section? NextSection { get; set; }
        public bool EndForm { get; set; }
        public bool EndSection { get; set; }
        public RadioOptionItem Option { get; set; }
    }

    public class CheckboxOptions
    {
        public Page? NextPage { get; set; }
        public Section? NextSection { get; set; }
        public bool EndForm { get; set; }
        public bool EndSection { get; set; }
        public RadioOptionItem Option { get; set; }
        public int? MinNumberOfOptions { get; set; }
        public int? MaxNumberOfOptions { get; set; }
    }

    public class NumberInputOptions
    {
        public int? GreaterThanOrEqualTo { get; set; }
        public int? LessThanOrEqualTo { get; set; }
        public int? NotEqualTo { get; set; }
    }

    public static GetQuestionByIdQueryResponse Map(Question entity, List<View_QuestionRoutingDetail> questionRoutes)
    {
        public int? GreaterThanOrEqualTo { get; set; }
        public int? LessThanOrEqualTo { get; set; }
        public int? NotEqualTo { get; set; }
    }

    public class DateInputOptions
    {
        public DateOnly? GreaterThanOrEqualTo { get; set; }
        public DateOnly? LessThanOrEqualTo { get; set; }
        public bool? MustBeInFuture { get; set; }
        public bool? MustBeInPast { get; set; }
    }

    public static implicit operator GetQuestionByIdQueryResponse(Question entity)
    {
        var question = new GetQuestionByIdQueryResponse()
        {
            Id = entity.Id,
            PageId = entity.PageId,
            Title = entity.Title,
            Key = entity.Key,
            Hint = entity.Hint,
            Order = entity.Order,
            Required = entity.Required,
            Type = entity.Type,
        };

        if (questionRoutes != null)
        {
            foreach (var option in questionRoutes)
            {
                question.Routes.Add(new()
                {
                    EndForm = option.EndForm,
                    EndSection = option.EndSection,
                    NextPage = new()
                    {
                        Id = option.NextPageId ?? default,
                        Order = option.NextPageOrder ?? default,
                        Title = option.NextPageTitle
                    },
                    NextSection = new()
                    {
                        Id = option.NextSectionId ?? default,
                        Order = option.NextSectionOrder ?? default,
                        Title = option.NextSectionTitle
                    },
                    Option = new()
                    {
                        Id = option.OptionId,
                        Order = option.OptionOrder,
                        Value = option.OptionValue
                    }
                });
            }
        }

        if (question.Type == QuestionType.Text.ToString() && entity.QuestionValidation != null)
        {
            question.TextInput = new()
            {
                MinLength = entity.QuestionValidation.MinLength,
                MaxLength = entity.QuestionValidation.MaxLength,
            };
        }

        else if ((question.Type == QuestionType.Radio.ToString() || question.Type == QuestionType.MultiChoice.ToString()) && entity.QuestionOptions != null)
        {
            question.Options = new();
            foreach (var option in entity.QuestionOptions)
            {
                question.Options.Add(new()
                {
                    Id = option.Id,
                    Value = option.Value,
                    Order = option.Order,
                });
            }


            if (question.Type == QuestionType.MultiChoice.ToString())
            {
                question.Checkbox = new()
                {
                    MaxNumberOfOptions = entity.QuestionValidation?.MaxNumberOfOptions ?? 0,
                    MinNumberOfOptions = entity.QuestionValidation?.MinNumberOfOptions ?? 0,
                };
            }
        }
        else if (question.Type == QuestionType.Number.ToString())
        {
            question.NumberInput = new()
            {
                GreaterThanOrEqualTo = entity.QuestionValidation?.NumberGreaterThanOrEqualTo,
                LessThanOrEqualTo = entity.QuestionValidation?.NumberLessThanOrEqualTo,
                NotEqualTo = entity.QuestionValidation?.NumberNotEqualTo,
            };
        }
        else if (question.Type == QuestionType.Date.ToString())
        {
            question.DateInput = new()
            {
                GreaterThanOrEqualTo = entity.QuestionValidation?.DateGreaterThanOrEqualTo,
                LessThanOrEqualTo = entity.QuestionValidation?.DateLessThanOrEqualTo,
                MustBeInFuture = entity.QuestionValidation?.DateMustBeInFuture,
                MustBeInPast = entity.QuestionValidation?.DateMustBeInPast,
            };
        }

        return question;
    }

}