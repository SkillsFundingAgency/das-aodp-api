using MediatR;using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQuery : IRequest<BaseMediatrResponse<GetAllSectionsQueryResponse>>
{
    public readonly Guid FormVersionId;

    public GetAllSectionsQuery(Guid formVersionId)
    {
        FormVersionId = formVersionId;
    }
}