using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetFormVersionByIdQueryResponse(FormVersion? data = null) : BaseResponse
{
    public FormVersion? Data { get; set; } = data;
}