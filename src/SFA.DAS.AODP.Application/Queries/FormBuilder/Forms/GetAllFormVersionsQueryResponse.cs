using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

public class GetAllFormVersionsQueryResponse(List<FormVersion> data) : BaseResponse
{
    public List<FormVersion> Data { get; set; } = data;
}