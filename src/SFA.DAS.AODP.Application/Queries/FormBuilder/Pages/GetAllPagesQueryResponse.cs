namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryResponse : BaseResponse
{
    public List<Page> Data { get; set; } = new List<Page>();

    public class Page
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid Key { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public int? NextPageId { get; set; }
    }
}
