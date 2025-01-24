using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

/// <exception cref="LockedRecordException"></exception>
/// <exception cref="DependantNotFoundException"></exception>
public class CreatePageCommandResponse : BaseResponse
{
    public Page Data { get; set; } = new Page();

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