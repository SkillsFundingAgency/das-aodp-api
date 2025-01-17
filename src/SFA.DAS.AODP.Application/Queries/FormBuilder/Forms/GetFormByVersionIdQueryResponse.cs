using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetFormByVersionIdQueryResponse : BaseResponse
{
    public FormVersion? Data { get; set; }
}