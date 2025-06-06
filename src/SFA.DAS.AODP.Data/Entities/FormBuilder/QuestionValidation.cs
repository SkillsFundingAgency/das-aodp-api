﻿namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class QuestionValidation
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }

    // Text
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }

    // Checkbox
    public int? MinNumberOfOptions { get; set; }
    public int? MaxNumberOfOptions { get; set; }

    // Number
    public int? NumberGreaterThanOrEqualTo { get; set; }
    public int? NumberLessThanOrEqualTo { get; set; }
    public int? NumberNotEqualTo { get; set; }

    // Date
    public DateOnly? DateGreaterThanOrEqualTo { get; set; }
    public DateOnly? DateLessThanOrEqualTo { get; set; }
    public bool? DateMustBeInFuture { get; set; }
    public bool? DateMustBeInPast { get; set; }

    // File
    public string? FileNamePrefix { get; set; }
    public int? NumberOfFiles { get; set; }


    public Question Question { get; set; }
}
