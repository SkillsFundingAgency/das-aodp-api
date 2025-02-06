using SFA.DAS.AODP.Application;

public class GetApplicationSectionByIdQueryResponse : BaseResponse
{
    public string SectionTitle { get; set; }

    public List<Page> Pages { get; set; } = new();

    public class Page
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
    }

    public static implicit operator GetApplicationSectionByIdQueryResponse(SFA.DAS.AODP.Data.Entities.FormBuilder.Section section)
    {
        GetApplicationSectionByIdQueryResponse response = new()
        {
            SectionTitle = section.Title
        };

        foreach (var page in section.Pages ?? [])
        {
            response.Pages.Add(new Page { Id = page.Id, Title = page.Title, Order = page.Order });
        }

        return response;
    }
}