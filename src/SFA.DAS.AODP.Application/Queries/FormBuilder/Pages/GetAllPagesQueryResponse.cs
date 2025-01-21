using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQueryResponse(List<Page> data) : BaseResponse
{
    public List<Page> Data { get; set; } = data;
}
