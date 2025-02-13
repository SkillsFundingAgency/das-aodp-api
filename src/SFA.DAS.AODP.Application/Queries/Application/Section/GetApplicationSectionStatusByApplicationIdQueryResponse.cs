using SFA.DAS.AODP.Data.Entities.Application;

public class GetApplicationSectionStatusByApplicationIdQueryResponse
{
    public List<Page> Pages { get; set; } = new();

    public class Page
    {
        public Guid PageId { get; set; }
        public string Status { get; set; }
    }

    public static implicit operator GetApplicationSectionStatusByApplicationIdQueryResponse(List<ApplicationPage> pages)
    {
        GetApplicationSectionStatusByApplicationIdQueryResponse response = new();

        foreach (ApplicationPage page in pages)
        {
            response.Pages.Add(new()
            {
                PageId = page.PageId,
                Status = page.Status,
            });
        }

        return response;
    }

}