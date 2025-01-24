using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

/// <exception cref="NotFoundException"></exception>
public class GetSectionByIdQueryResponse : BaseResponse
{
    public Section Data { get; set; } = new Section();

    public class Section
    {
        public Guid Id { get; set; }
        public Guid FormVersionId { get; set; }
        public Guid Key { get; set; }
        public int Order { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? NextSectionId { get; set; }
    }
}