﻿using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

public class GetApplicationPageByIdQueryResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public int TotalSectionPages { get; set; }

    public List<Question> Questions { get; set; }
    public Guid SectionId { get; private set; }
    public class RouteInformation
    {
        public Guid OptionId { get; set; }
        public int? NextPageOrder { get; set; }
        public int? NextSectionOrder { get; set; }
        public bool EndForm { get; set; }
        public bool EndSection { get; set; }
    }

    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; }
        public bool Required { get; set; }
        public string? Hint { get; set; } = string.Empty;
        public int Order { get; set; }

        public TextInputOptions TextInput { get; set; } = new();
        public RadioOptions RadioButton { get; set; } = new();

        public List<RouteInformation> Routes { get; set; } = new();



        public static implicit operator Question(SFA.DAS.AODP.Data.Entities.FormBuilder.Question entity)
        {
            var model = new Question()
            {
                Id = entity.Id,
                Title = entity.Title,
                Hint = entity.Hint,
                Order = entity.Order,
                Required = entity.Required,
                Type = entity.Type,
            };

            if (entity.Type == QuestionType.Text.ToString() && entity.QuestionValidation != null)
            {
                model.TextInput = new()
                {
                    MinLength = entity.QuestionValidation.MinLength,
                    MaxLength = entity.QuestionValidation.MaxLength,
                };
            }

            else if (entity.Type == QuestionType.Radio.ToString() && entity.QuestionOptions != null)
            {
                model.RadioButton = new();
                foreach (var option in entity.QuestionOptions)
                {
                    model.RadioButton.MultiChoice.Add(new()
                    {
                        Id = option.Id,
                        Value = option.Value,
                        Order = option.Order,
                    });
                }

                if (entity.Routes != null && entity.Routes.Count > 0)
                {
                    foreach (var route in entity.Routes)
                    {
                        model.Routes.Add(new()
                        {
                            EndForm = route.EndForm,
                            EndSection = route.EndSection,
                            NextPageOrder = route.NextPage?.Order,
                            NextSectionOrder = route.NextSection?.Order,
                            OptionId = route.SourceOptionId
                        });
                    }
                }
            }

            return model;
        }

    }

    public class TextInputOptions
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }

    }

    public class RadioOptions
    {
        public List<RadioOptionItem> MultiChoice { get; set; } = new();

        public class RadioOptionItem
        {
            public Guid Id { get; set; }
            public string Value { get; set; }
            public int Order { get; set; }
        }
    }


    public static implicit operator GetApplicationPageByIdQueryResponse(Page entity)
    {
        return new()
        {
            Id = entity.Id,
            SectionId = entity.SectionId,
            Title = entity.Title,
            Order = entity.Order,
            Questions = entity.Questions != null ? [.. entity.Questions] : new(),
            TotalSectionPages = entity.Section.View_SectionPageCount.PageCount
        };
    }
}
