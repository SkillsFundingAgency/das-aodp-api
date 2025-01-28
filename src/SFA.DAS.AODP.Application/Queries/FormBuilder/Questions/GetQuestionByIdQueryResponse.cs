using Newtonsoft.Json;
using SFA.DAS.AODP.Data.Entities;

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


    public class TextInputOptions
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }
    public class RadioOptionItem
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }

    public static implicit operator GetQuestionByIdQueryResponse(Data.Entities.Question entity)
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

        if (question.Type == QuestionType.Text.ToString() && !string.IsNullOrWhiteSpace(entity.TextValidator))
        {
            question.TextInput = JsonConvert.DeserializeObject<TextInputOptions>(entity.TextValidator);
        }

        else if (question.Type == QuestionType.Radio.ToString() && !string.IsNullOrWhiteSpace(entity.RadioValidator))
        {
            question.RadioOptions = JsonConvert.DeserializeObject<List<RadioOptionItem>>(entity.RadioValidator);
        }

        return question;
    }

}