using Azure;
using static SFA.DAS.AODP.Data.Repositories.Application.ApplicationQuestionAnswerRepository;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationDetailsByIdQueryResponse
{
    public Guid ApplicationId { get; set; }
    public List<Question> QuestionsWithAnswers { get; set; } = new List<Question>();

    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; }
        public bool Required { get; set; }
        public List<QuestionAnswer>? QuestionAnswers { get; set; } = new List<QuestionAnswer>();
    }

    public class QuestionAnswer
    {
        public string? AnswerTextValue { get; set; }
        public string? AnswerDateValue { get; set; }
        public string? AnswerChoiceValue { get; set; }
        public decimal? AnswerNumberValue { get; set; }
    }


    public static GetApplicationDetailsByIdQueryResponse Map(
       Guid applicationId,
       List<ApplicationQuestionAnswersDTO> answers)
    {
        return new GetApplicationDetailsByIdQueryResponse
        {
            ApplicationId = applicationId,
            QuestionsWithAnswers = answers
                    .GroupBy(q => new { q.QuestionId, q.QuestionTitle, q.QuestionType, q.QuestionRequired })
                    .Select(questionGroup => new Question
                    {
                        Id = questionGroup.Key.QuestionId,
                        Title = questionGroup.Key.QuestionTitle,
                        Type = questionGroup.Key.QuestionType,
                        Required = questionGroup.Key.QuestionRequired,
                        QuestionAnswers = questionGroup
                            .Select(a => new QuestionAnswer
                            {
                                AnswerTextValue = a.AnswerText,
                                AnswerDateValue = a.AnswerDate,
                                AnswerChoiceValue = a.AnswerChoice,
                                AnswerNumberValue = a.AnswerNumber
                            })
                            .ToList()
                    })
                    .ToList()
          };
    }
}
