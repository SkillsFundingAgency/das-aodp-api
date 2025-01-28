using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;


/// <exception cref="NotFoundException"></exception>
public class GetFormVersionByIdQueryResponse : BaseResponse
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string Title { get; set; }
    public DateTime Version { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public List<Section> Sections { get; set; }

    public static implicit operator GetFormVersionByIdQueryResponse(Data.Entities.FormVersion formVersion)
    {
        return new GetFormVersionByIdQueryResponse()
        {
            Id = formVersion.Id,
            FormId = formVersion.FormId,
            Description = formVersion.Description,
            Order = formVersion.Order,
            Title = formVersion.Title,
            Version = formVersion.Version,
            Status = formVersion.Status,
            Sections = [.. formVersion.Sections]

        };
    }


    public class Section
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }

        public static implicit operator Section(Data.Entities.Section section)
        {
            return new()
            {
                Id = section.Id,
                Key = section.Key,
                Order = section.Order,
                Title = section.Title
            };
        }
    }



}