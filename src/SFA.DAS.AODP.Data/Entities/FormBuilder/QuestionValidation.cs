namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class QuestionValidation
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }

    public Question Question { get; set; }
}
