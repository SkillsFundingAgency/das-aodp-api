namespace SFA.DAS.AODP.Data.Entities;

public class Option
{
    public Guid Id { get; set; }
    public int QuestionId { get; set; }
    public string Value { get; set; }
    public string Text { get; set; }
    public int Order { get; set; }
}