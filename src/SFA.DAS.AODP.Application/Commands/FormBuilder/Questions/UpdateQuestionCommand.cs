﻿using MediatR;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Question;

public class UpdateQuestionCommand : IRequest<BaseMediatrResponse<EmptyResponse>>
{
    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
    public Guid PageId { get; set; }
    public string Title { get; set; }
    public string? Hint { get; set; }
    public bool Required { get; set; }
    public TextInputOptions TextInput { get; set; } = new();
    public NumberInputOptions NumberInput { get; set; } = new();
    public CheckboxOptions Checkbox { get; set; } = new();
    public List<OptionItem> Options { get; set; } = new();
    public class TextInputOptions
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }

    public class OptionItem
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
    }

    public class CheckboxOptions
    {
        public int? MinNumberOfOptions { get; set; }
        public int? MaxNumberOfOptions { get; set; }
    }

    public class NumberInputOptions
    {
        public int? GreaterThanOrEqualTo { get; set; }
        public int? LessThanOrEqualTo { get; set; }
        public int? NotEqualTo { get; set; }
    }
}