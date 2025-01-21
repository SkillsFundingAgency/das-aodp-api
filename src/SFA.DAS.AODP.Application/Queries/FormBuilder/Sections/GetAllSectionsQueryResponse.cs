using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQueryResponse(List<Section> data) : BaseResponse
{
    public List<Section> Data { get; set; } = data;
}
