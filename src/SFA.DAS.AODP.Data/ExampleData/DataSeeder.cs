using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Data.ExampleData;

public static class DataSeeder
{
    public static async Task SeedAsync(DbContext context, int seedCount = 20)
    {
        Guid NewGuid() => Guid.NewGuid();

        try
        {
            Console.WriteLine("Starting seeding process...");

            var forms = new List<Form>();
            var formVersions = new List<FormVersion>();
            //var sections = new List<Section>();
            //var pages = new List<Page>();
            //var questions = new List<Question>();
            //var validationRules = new List<ValidationRule>();
            //var options = new List<Option>();

            for (int i = 1; i <= seedCount; i++)
            {
                var formId = NewGuid();
                Random random = new Random();
                bool isArchived = random.Next(0, 2) == 0;
                var form = new Form { Id = NewGuid(), Archived = isArchived };
                context.Set<Form>().Add(form);


                //var questionOptions = CreateOptions(4);
                //var formValidationRules = CreateValidationRules();
                //var formSections = CreateSections(formId, questionOptions, formValidationRules);

                //foreach (var section in formSections)
                //{
                //    sections.Add(section);
                //    foreach (var page in section.Pages)
                //    {
                //        pages.Add(page);
                //        foreach (var question in page.Questions)
                //        {
                //            questions.Add(question);
                //            validationRules.AddRange(question.ValidationRules);
                //            options.AddRange(question.Options);
                //        }
                //    }
                //}

                context.Set<FormVersion>().Add(new()
                {
                    DateCreated = DateTime.Now,
                    FormId = form.Id,
                    Description = "Something",
                    Status = FormStatus.Draft,
                    Name = "Name",
                    Version = DateTime.Now,
                });

                //context.Set<Section>().AddRange(sections);
                //context.Set<Page>().AddRange(pages);
                //context.Set<Question>().AddRange(questions);
                //context.Set<Option>().AddRange(options);

                Console.WriteLine($"Created Form {i} with ID: {formId}");
            }

            Console.WriteLine("Seeding data into cache...");

            await context.SaveChangesAsync();

            Console.WriteLine("Seeding process completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seeding error: {ex.Message}");
        }
    }

    private static List<Option> CreateOptions(int count)
    {
        var options = new List<Option>();
        for (int i = 1; i <= count; i++)
        {
            options.Add(new Option
            {
                Id = Guid.NewGuid(),
                Text = $"Option {i}",
                Value = i.ToString()
            });
        }
        return options;
    }

    private static List<ValidationRule> CreateValidationRules()
    {
        return new List<ValidationRule>
        {
            new ValidationRule
            {
                Id = Guid.NewGuid(),
                Type = "Required",
                Value = "true",
                ErrorMessage = "This field is required."
            }
        };
    }

    private static List<Section> CreateSections(Guid formId, List<Option> options, List<ValidationRule> validationRules)
    {
        var sections = new List<Section>();
        for (int i = 1; i <= 3; i++)
        {
            var sectionId = Guid.NewGuid();
            var pages = CreatePages(sectionId, options, validationRules);

            sections.Add(new Section
            {
                Id = sectionId,
                FormVersionId = formId,
                Title = $"Sample Section {i}",
                Description = $"Section {i} description.",
                Order = i,
                Pages = pages
            });
        }
        return sections;
    }

    private static List<Page> CreatePages(Guid sectionId, List<Option> options, List<ValidationRule> validationRules)
    {
        var pages = new List<Page>();
        for (int i = 1; i <= 3; i++)
        {
            var pageId = Guid.NewGuid();
            var questions = CreateQuestions(pageId, options, validationRules);

            pages.Add(new Page
            {
                Id = pageId,
                SectionId = sectionId,
                Title = $"Sample Page {i}",
                Description = $"Page {i} description.",
                Order = i,
                Questions = questions
            });
        }
        return pages;
    }

    private static List<Question> CreateQuestions(Guid pageId, List<Option> options, List<ValidationRule> validationRules)
    {
        var questions = new List<Question>();
        for (int i = 1; i <= 3; i++)
        {
            questions.Add(new Question
            {
                Id = Guid.NewGuid(),
                PageId = pageId,
                Title = $"Sample Question {i}",
                Type = "Text",
                Required = true,
                Order = i,
                Description = $"Describe something for question {i}.",
                Hint = $"Hint text for question {i}",
                Options = options,
                ValidationRules = validationRules
            });
        }
        return questions;
    }
}
