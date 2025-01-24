namespace SFA.DAS.AODP.Data.Entities;

public class ValidationRule
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public List<Question> Questions { get; set; } = new List<Question>();
}
