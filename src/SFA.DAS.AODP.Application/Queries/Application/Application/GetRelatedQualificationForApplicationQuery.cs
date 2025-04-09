using MediatR;
using SFA.DAS.AODP.Application;

public class GetRelatedQualificationForApplicationQuery : IRequest<BaseMediatrResponse<GetRelatedQualificationForApplicationQueryResponse>>
{
    public GetRelatedQualificationForApplicationQuery(Guid applicationId)
    {
        ApplicationId = applicationId;
    }
    public Guid ApplicationId { get; set; }
}
