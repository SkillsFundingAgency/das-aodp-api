namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQueryResponse()
{
    public List<Section> Data { get; set; }

    public class Section
    {
        public Guid Id { get; set; }
        public Guid FormVersionId { get; set; }
        public Guid Key { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }


        public static implicit operator Section(Data.Entities.FormBuilder.Section entity)
        {
            return (new()
            {
                Id = entity.Id,
                Title = entity.Title,
                Order = entity.Order,
                FormVersionId = entity.FormVersionId,
                Key = entity.Key
            });
        }
    }
}
