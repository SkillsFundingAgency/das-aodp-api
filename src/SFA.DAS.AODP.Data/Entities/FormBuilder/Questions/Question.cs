﻿using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities;

public class Question
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public string Title { get; set; } = string.Empty;
    [Column(TypeName = "nvarchar(100)")]
    public QuestionType Type { get; set; }
    public bool Required { get; set; }
    public int Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public List<Option> MultiChoice { get; set; } = new List<Option>();
    public List<RoutingPoint> RoutingPoints { get; set; } = new List<RoutingPoint>();
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
