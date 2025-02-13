namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class QuestionOption
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Value { get; set; }
    public int Order { get; set; }
}
