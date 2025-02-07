using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetSectionByIdQueryResponse() : BaseResponse
{
    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid Key { get; set; }
    public int Order { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<Page> Pages { get; set; }
    public bool Editable { get; set; }

    public static implicit operator GetSectionByIdQueryResponse(Section entity)
    {
        return new()
        {
            Id = entity.Id,
            FormVersionId = entity.FormVersionId,
            Title = entity.Title,
            Key = entity.Key,
            Description = entity.Description,
            Order = entity.Order,
            Pages = entity.Pages != null ? [.. entity.Pages] : new()

        };

    }


    public class Page
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }

        public static implicit operator Page(Data.Entities.FormBuilder.Page entity)
        {
            return new()
            {
                Id = entity.Id,
                Key = entity.Key,
                Order = entity.Order,
                Title = entity.Title
            };
        }
    }
}