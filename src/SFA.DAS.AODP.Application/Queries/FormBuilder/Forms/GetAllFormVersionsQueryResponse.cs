using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetAllFormVersionsQueryResponse : BaseResponse
{
    public List<FormVersion> Data { get; set; } = new List<FormVersion>();

    public class FormVersion
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Version { get; set; }
        public FormStatus Status { get; set; }
        public bool Archived { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime DateCreated { get; set; }
    }
}