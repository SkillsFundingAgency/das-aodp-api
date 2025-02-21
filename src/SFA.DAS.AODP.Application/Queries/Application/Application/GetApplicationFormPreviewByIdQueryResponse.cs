namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationFormPreviewByIdQueryResponse
{
    public Guid ApplicationId { get; set; }
    public Guid FormVersionId { get; set; }
    public List<Question> Data { get; set; } = new List<Question>();

    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public static implicit operator Question(Data.Entities.FormBuilder.Question entity)
        {
            return (new()
            {
                Id = entity.Id,
                Title = entity.Title
            });
        }

    }
}
