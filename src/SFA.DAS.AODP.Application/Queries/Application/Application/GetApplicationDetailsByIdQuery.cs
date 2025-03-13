using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;
public class GetApplicationDetailsByIdQuery : IRequest<BaseMediatrResponse<GetApplicationDetailsByIdQueryResponse>>
{
    public GetApplicationDetailsByIdQuery(Guid applicationId)
    {
        ApplicationId = applicationId;
    }
    public Guid ApplicationId { get; set; }
}
