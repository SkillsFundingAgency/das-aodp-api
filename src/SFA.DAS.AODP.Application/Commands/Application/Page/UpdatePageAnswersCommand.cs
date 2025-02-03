using MediatR;

public class UpdatePageAnswersCommand : IRequest<UpdatePageAnswersCommandResponse>
{
    public Guid ApplicationId { get; set; }
    public Guid PageId { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
    public List<Question> Questions { get; set; } = new();

    public class Question
    {
        public Guid QuestionId { get; set; }
        public string QuestionType { get; set; }
        public Answer Answer { get; set; }
    }

    public class Answer
    {
        public string? TextValue { get; set; }
        public int? IntegerValue { get; set; }
        public DateTime? DateValue { get; set; }
        public List<string>? MultipleChoiceValue { get; set; }
        public string? RadioChoiceValue { get; set; }
    }

}