using SFA.DAS.AODP.Application.Exceptions;

using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;

/// <exception cref="NotFoundException"></exception>
public class UpdateFormVersionCommandResponse : BaseResponse
{
    public FormVersion Data { get; set; } = new FormVersion();

    public class FormVersion
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Version { get; set; }
        public FormStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime DateCreated { get; set; }
    }
}