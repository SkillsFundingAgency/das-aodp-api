﻿using SFA.DAS.AODP.Models.Forms.Validators;

namespace SFA.DAS.AODP.Models.Forms.FormSchema;

public class QuestionSchema
{
    public int Id { get; set; }
    public int PageId { get; set; }
    public int SectionId { get; set; }
    public int FormId { get; set; }
    public int Index { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public List<string> MultiChoice { get; set; } = new List<string>();
    public Dictionary<string, RoutingPoint> RoutingPoints { get; set; } = new Dictionary<string, RoutingPoint>();
    public TextValidator? TextValidator { get; set; }
    public IntegerValidator? IntegerValidator { get; set; }
    public DecimalValidator? DecimalValidator { get; set; }
    public DateValidator? DateValidator { get; set; }
    public MultiChoiceValidator? MultiChoiceValidator { get; set; }
    public BooleanValidaor? BooleanValidaor { get; set; }
}

public enum QuestionType
{
    Text,        // Not null, length min, length max 
    Integer,     // Not null, greater than, less than, equal/not equal to 
    Decimal,     // Not null, greater than, less than, equal/not equal to 
    Date,        // Not null, greater than, less than, equal/not equal to 
    MultiChoice, // Not null
    Boolean      // Not null
}
