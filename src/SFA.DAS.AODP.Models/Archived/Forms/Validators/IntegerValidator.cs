﻿namespace SFA.DAS.AODP.Models.Archived.Forms.Validators;

public class IntegerValidator
{
    public bool Required { get; set; }
    public int? GreaterThan { get; set; }
    public int? LessThan { get; set; }
    public int? EqualTo { get; set; }
    public int? NotEqualTo { get; set; }
}
