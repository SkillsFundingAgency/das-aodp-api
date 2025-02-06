using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.Application;

public class GetApplicationPageAnswersByPageIdQueryResponse : BaseResponse
{
    public List<Question> Questions { get; set; } = new();

    public class Question
    {
        public Guid QuestionId { get; set; }
        public Answer Answer { get; set; }
    }

    public class Answer
    {
        public string? TextValue { get; set; }
        public decimal? NumberValue { get; set; }
        public DateTime? DateValue { get; set; }
        public List<string>? MultipleChoiceValue { get; set; }
        public string? RadioChoiceValue { get; set; }
    }

    public static implicit operator GetApplicationPageAnswersByPageIdQueryResponse(List<ApplicationQuestionAnswer> questions)
    {
        GetApplicationPageAnswersByPageIdQueryResponse response = new();

        foreach (var quesiton in questions ?? [])
        {
            response.Questions.Add(new Question
            {
                QuestionId = quesiton.QuestionId,
                Answer = new()
                {
                    DateValue = quesiton.DateTimeValue,
                    TextValue = quesiton.TextValue,
                    NumberValue = quesiton.NumberValue,
                    MultipleChoiceValue = quesiton.OptionsValue?.Split(",")?.ToList() ?? [],
                    RadioChoiceValue = quesiton.OptionsValue
                }
            });
        }

        return response;
    }
}