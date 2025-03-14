using Azure;
using static SFA.DAS.AODP.Data.Repositories.Application.ApplicationQuestionAnswerRepository;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationDetailsByIdQueryResponse
{
    public Guid ApplicationId { get; set; }
    public List<Section> SectionsWithPagesAndQuestionsAndAnswers { get; set; } = new List<Section>();

    public class Section
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public List<Page> Pages { get; set; } = new List<Page>();
    }

    public class Page
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }

    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; }
        public bool Required { get; set; }
        public List<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();
    }

    public class QuestionAnswer
    {
        public string AnswerText { get; set; }
    }

    public static GetApplicationDetailsByIdQueryResponse Map(
       Guid applicationId,
       List<ApplicationQuestionAnswersDTO> answers)
    {
        return new GetApplicationDetailsByIdQueryResponse
        {
            ApplicationId = applicationId,
            SectionsWithPagesAndQuestionsAndAnswers = answers
                .GroupBy(s => new { s.SectionId, s.SectionTitle, s.SectionOrder })
                .Select(sectionGroup => new Section
                {
                    Id = sectionGroup.Key.SectionId,
                    Order = sectionGroup.Key.SectionOrder,
                    Title = sectionGroup.Key.SectionTitle,
                    Pages = sectionGroup
                        .GroupBy(p => new { p.PageId, p.PageTitle, p.PageOrder })
                        .Select(pageGroup => new Page
                        {
                            Id = pageGroup.Key.PageId,
                            Order = pageGroup.Key.PageOrder,
                            Title = pageGroup.Key.PageTitle,
                            Questions = pageGroup
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
                                            AnswerText = a.AnswerText
                                        })
                                        .ToList()
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList()
          };
    }
}
