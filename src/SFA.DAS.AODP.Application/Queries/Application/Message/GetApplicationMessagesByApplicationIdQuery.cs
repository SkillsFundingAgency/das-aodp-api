using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Application.Message;

public class GetApplicationMessagesByApplicationIdQuery : IRequest<BaseMediatrResponse<GetApplicationMessagesByApplicationIdQueryResponse>>
{
    public GetApplicationMessagesByApplicationIdQuery(Guid applicationId, string userType)
    {
        ApplicationId = applicationId;
        UserType = userType;
    }
    public Guid ApplicationId { get; set; }
    public string UserType { get; set; }
}
