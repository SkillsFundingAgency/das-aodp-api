using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

/// <exception cref="NotFoundException"></exception>
public class GetPageByIdQueryResponse : BaseResponse
{
    public Page Data { get; set; } = new();

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