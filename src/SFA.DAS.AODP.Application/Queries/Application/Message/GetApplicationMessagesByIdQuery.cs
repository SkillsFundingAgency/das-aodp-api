using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Application.Message;

public class GetApplicationMessagesByIdQuery : IRequest<BaseMediatrResponse<GetApplicationMessagesByIdQueryResponse>>
{
    public GetApplicationMessagesByIdQuery(Guid applicationId)
    {
        ApplicationId = applicationId;
    }
    public Guid ApplicationId { get; set; }
}
