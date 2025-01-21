namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryResponse(List<Page> data) : BaseResponse
{
<<<<<<< HEAD
    public List<Page> Data { get; set; } = data;
}
=======
    public List<Page> Data { get; set; }

    public class Page
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string Title { get; set; }
        public Guid Key { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public int? NextPageId { get; set; }
    }
}
>>>>>>> ab4b648a65944bfdc978e773d93dae3cd911f872
