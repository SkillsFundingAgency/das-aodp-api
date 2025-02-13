using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

public class GetAllPagesQuery : IRequest<BaseMediatrResponse<GetAllPagesQueryResponse>>
{
    public readonly Guid SectionId;

    public GetAllPagesQuery(Guid sectionId)
    {
        SectionId = sectionId;
    }
}