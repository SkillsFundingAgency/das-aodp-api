﻿using SFA.DAS.AODP.Models.Forms.Application;
using SFA.DAS.AODP.Models.Forms.FormSchema;
using SFA.DAS.AODP.Models.Exceptions.FormValidation;

namespace SFA.DAS.AODP.Models.Forms.Validators;

public class DecimalValidator
{
    public bool Required { get; set; }
    public float? GreaterThan { get; set; }
    public float? LessThan { get; set; }
    public float? EqualTo { get; set; }
    public float? NotEqualTo { get; set; }
}
