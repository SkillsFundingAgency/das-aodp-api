using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetFormVersionByIdQueryResponse(FormVersion? data = null) : BaseResponse
{
<<<<<<< HEAD
    public FormVersion? Data { get; set; } = data;
=======
    public FormVersion? Data { get; set; }

    public class FormVersion
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public string Name { get; set; }
        public DateTime Version { get; set; }
        public FormStatus Status { get; set; }
        public bool Archived { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public DateTime DateCreated { get; set; }
    }
>>>>>>> ab4b648a65944bfdc978e773d93dae3cd911f872
}