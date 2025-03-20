namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationFormPreviewByIdQueryResponse
{
    public Guid ApplicationId { get; set; }
    public Guid FormVersionId { get; set; }
    public List<Section> SectionsWithPagesAndQuestions { get; set; } = new List<Section>();

    public class Section
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public List<Page> Pages { get; set; } = new List<Page>();
    }

    public class Page
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
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
        public List<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
    }

    public class QuestionOption
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }

    public static GetApplicationFormPreviewByIdQueryResponse Map(
           Guid applicationId,
           Guid formVersionId,
           List<SFA.DAS.AODP.Data.Entities.FormBuilder.Section> sections)
    {
        return new GetApplicationFormPreviewByIdQueryResponse
        {
            ApplicationId = applicationId,
            FormVersionId = formVersionId,
            SectionsWithPagesAndQuestions = sections.Select(s => new Section
            {
                Id = s.Id,
                Key = s.Key,
                Order = s.Order,
                Title = s.Title,
                Pages = s.Pages.Select(p => new Page
                {
                    Id = p.Id,
                    Key = p.Key,
                    Order = p.Order,
                    Title = p.Title,
                    Questions = p.Questions.Select(q => new Question
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Type = q.Type,
                        Required = q.Required,
                        QuestionOptions = q.QuestionOptions?.Select(opt => new QuestionOption
                        {
                            Id = opt.Id,
                            Value = opt.Value
                        }).ToList() ?? new List<QuestionOption>()
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }
}
