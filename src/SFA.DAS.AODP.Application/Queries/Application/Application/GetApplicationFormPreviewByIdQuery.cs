using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Application.Application;

public class GetApplicationFormPreviewByIdQuery : IRequest<BaseMediatrResponse<GetApplicationFormPreviewByIdQueryResponse>>
{
    public Guid ApplicationId { get; set; }

    public GetApplicationFormPreviewByIdQuery(Guid applicationId)
    {
        ApplicationId = applicationId;
    }
}
