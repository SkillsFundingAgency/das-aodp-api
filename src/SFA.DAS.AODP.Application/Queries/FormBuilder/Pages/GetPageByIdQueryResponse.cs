namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetPageByIdQueryResponse() : BaseResponse
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Title { get; set; }
    public Guid Key { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public List<Question> Questions { get; set; }

    public static implicit operator GetPageByIdQueryResponse(Data.Entities.Page entity)
    {
        return new()
        {
            Id = entity.Id,
            SectionId = entity.SectionId,
            Title = entity.Title,
            Key = entity.Key,
            Description = entity.Description,
            Order = entity.Order,
            Questions = entity.Questions != null ? [.. entity.Questions] : new()

        };
    }

    public class Question
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }

        public static implicit operator Question(Data.Entities.Question question)
        {
            return new()
            {
                Id = question.Id,
                Key = question.Key,
                Order = question.Order,
                Title = question.Title
            };
        }
    }
}

