﻿using SFA.DAS.AODP.Models.Forms.Application;
using SFA.DAS.AODP.Models.Forms.FormSchema;
using SFA.DAS.AODP.Models.Exceptions.FormValidation;

namespace SFA.DAS.AODP.Models.Forms.Validators;

public class IntegerValidator
{
    public bool Required { get; set; }
    public int? GreaterThan { get; set; }
    public int? LessThan { get; set; }
    public int? EqualTo { get; set; }
    public int? NotEqualTo { get; set; }
}
