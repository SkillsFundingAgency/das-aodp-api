using SFA.DAS.AODP.Models.Forms.FormBuilder;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetSectionByIdQueryResponse(Section data) : BaseResponse
{
    public Section Data { get; set; } = data;
}