﻿namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class Question
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; }
    public bool Required { get; set; }
    public int Order { get; set; }
    public string? Hint { get; set; } = string.Empty;
    public string? Helper { get; set; } = string.Empty;
    public string? HelperHTML { get; set; } = string.Empty;
    public Guid Key { get; set; }

    public virtual Page Page { get; set; }
    public virtual QuestionValidation QuestionValidation { get; set; }
    public virtual List<QuestionOption> QuestionOptions { get; set; }
    public virtual List<Route> Routes { get; set; }
}

public enum QuestionType
{
    Text,
    TextArea,
    Number,
    Date,
    MultiChoice,
    Radio,
    File
}

