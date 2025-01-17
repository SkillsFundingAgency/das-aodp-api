using MediatR;

namespace SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

public class GetAllSectionsQueryRequest : IRequest<GetAllSectionsQueryResponse>
{
    public Guid FormId { get; set; }
}
