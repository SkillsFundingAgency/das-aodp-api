using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities;

public class Question
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; }
    public bool Required { get; set; }
    public int Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Hint { get; set; } = string.Empty;
    public string? MultiChoice { get; set; }
    public string? TextValidator { get; set; }
    public string? IntegerValidator { get; set; }
    public string? DecimalValidator { get; set; }
    public string? DateValidator { get; set; }
    public string? MultiChoiceValidator { get; set; }
    public string? RadioValidator { get; set; }
    public string? BooleanValidaor { get; set; }
    public Guid Key { get; set; }

    public virtual Page Page { get; set; }
}

public enum QuestionType
{
    Text,        // Not null, length min, length max 
    Integer,     // Not null, greater than, less than, equal/not equal to 
    Decimal,     // Not null, greater than, less than, equal/not equal to 
    Date,        // Not null, greater than, less than, equal/not equal to 
    MultiChoice, // Not null
    Boolean,      // Not null
    Radio
}
