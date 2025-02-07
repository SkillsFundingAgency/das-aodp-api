using Newtonsoft.Json;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Questions;

public class GetQuestionByIdQueryResponse() : BaseResponse
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
    public List<RadioOptionItem> RadioOptions { get; set; } = new();

    public bool Editable { get; set; }
    public class TextInputOptions
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }
    public class RadioOptionItem
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
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

        if (question.Type == QuestionType.Text.ToString() && entity.QuestionValidation != null)
        {
            question.TextInput = new()
            {
                MinLength = entity.QuestionValidation.MinLength,
                MaxLength = entity.QuestionValidation.MaxLength,
            };
        }

        else if (question.Type == QuestionType.Radio.ToString() && entity.QuestionOptions != null)
        {
            question.RadioOptions = new();
            foreach (var option in entity.QuestionOptions)
            {
                question.RadioOptions.Add(new()
                {
                    Id = option.Id,
                    Value = option.Value,
                    Order = option.Order,
                });
            }
        }

        return question;
    }

}