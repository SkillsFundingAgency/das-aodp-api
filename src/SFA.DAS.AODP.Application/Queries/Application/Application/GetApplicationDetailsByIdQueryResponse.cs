using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationDetailsByIdQueryResponse
{
    public Guid ApplicationId { get; set; }
    public List<Question> QuestionsWithAnswers { get; set; } = new List<Question>();

    public class Question
    {
        public Guid Id { get; set; }
        public Answer? Answer { get; set; }
    }

    public class Answer
    {
        public string? TextValue { get; set; }
        public decimal? NumberValue { get; set; }
        public DateOnly? DateValue { get; set; }
        public List<string>? MultipleChoiceValue { get; set; }
        public string? RadioChoiceValue { get; set; }
    }


    public static GetApplicationDetailsByIdQueryResponse Map(
       Guid applicationId,
       List<ApplicationQuestionAnswer> answers)
    {
        GetApplicationDetailsByIdQueryResponse response = new()
        {
            ApplicationId = applicationId,
        };

        foreach (var answer in answers)
        {
            response.QuestionsWithAnswers.Add(new()
            {
                Id = answer.QuestionId,
                Answer = new()
                {
                    DateValue = answer.DateValue,
                    TextValue = answer.TextValue,
                    NumberValue = answer.NumberValue,
                    MultipleChoiceValue = answer.OptionsValue?.Split(",")?.ToList() ?? [],
                    RadioChoiceValue = answer.OptionsValue
                }
            });
        }

        return response;
    }
}
