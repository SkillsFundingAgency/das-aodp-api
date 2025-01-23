using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Data.ExampleData;

public static class DataSeeder
{
    public static async Task SeedAsync(DbContext context, int seedCount = 20)
    {
#if (DEBUG)
        // Continue 
#else
        return;
#endif

        if (context.Set<Form>().Any())
            return;

        try
        {
            Console.WriteLine("Starting seeding process...");



            for (int i = 1; i <= seedCount; i++)
            {
                var forms = new List<Form>();
                var formVersions = new List<FormVersion>();
                var sections = new List<Section>();
                var pages = new List<Page>();

                Random random = new Random();
                bool isArchived = random.Next(0, 2) == 0;
                var form = new Form { Id = Guid.NewGuid(), Archived = isArchived };
                context.Set<Form>().Add(form);

                var formVersion = new FormVersion()
                {
                    Id = Guid.NewGuid(),
                    DateCreated = DateTime.Now,
                    FormId = form.Id,
                    Description = "Something",
                    Status = FormStatus.Draft,
                    Name = "Name",
                    Version = DateTime.Now,
                };
                context.Set<FormVersion>().Add(formVersion);
                await context.SaveChangesAsync();

                var formSections = CreateSections(formVersion.Id);

                foreach (var section in formSections)
                {
                    section.FormVersionId = formVersion.Id;
                    sections.Add(section);
                    foreach (var page in section.Pages)
                    {
                        page.SectionId = section.Id;
                        pages.Add(page);
                    }
                }

                context.Set<Section>().AddRange(sections);
                context.Set<Page>().AddRange(pages);

                Console.WriteLine($"Seeding Form  with ID `{form.Id}` into the DB ");
                await context.SaveChangesAsync();
            }

            Console.WriteLine("Seeding process completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seeding error: {ex.Message}");
        }
    }

    private static List<Section> CreateSections(Guid formId)
    {
        var sections = new List<Section>();
        for (int i = 1; i <= 3; i++)
        {
            var sectionId = Guid.NewGuid();
            var pages = CreatePages(sectionId);

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

    private static List<Page> CreatePages(Guid sectionId)
    {
        var pages = new List<Page>();
        for (int i = 1; i <= 3; i++)
        {
            var pageId = Guid.NewGuid();
            var questions = CreateQuestions(pageId);

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

    private static List<Question> CreateQuestions(Guid pageId)
    {
        var questions = new List<Question>();
        for (int i = 1; i <= 3; i++)
        {
            questions.Add(new Question
            {
                Id = Guid.NewGuid(),
                PageId = pageId,
                Title = $"Sample Question {i}",
                Type = QuestionType.Text,
                Required = true,
                Order = i,
                Description = $"Describe something for question {i}.",
                Hint = $"Hint text for question {i}"
            });
        }
        return questions;
    }
}
