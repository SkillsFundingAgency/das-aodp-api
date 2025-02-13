﻿namespace SFA.DAS.AODP.Models.Archived.Forms.Validators;

public class DecimalValidator
{
    public bool Required { get; set; }
    public float? GreaterThan { get; set; }
    public float? LessThan { get; set; }
    public float? EqualTo { get; set; }
    public float? NotEqualTo { get; set; }
}
