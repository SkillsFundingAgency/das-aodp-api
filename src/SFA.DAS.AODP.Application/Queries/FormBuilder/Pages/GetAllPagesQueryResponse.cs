namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryResponse
{
    public List<Page> Data { get; set; } = new List<Page>();

    public class Page
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid Key { get; set; }
        public int Order { get; set; }

        public static implicit operator Page(Data.Entities.FormBuilder.Page entity)
        {
            return (new()
            {
                Id = entity.Id,
                Title = entity.Title,
                Order = entity.Order,
                SectionId = entity.SectionId,
                Key = entity.Key
            });
        }

    }
}
