using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Qualification;

public class GetDiscussionHistoriesForQualificationQuery : IRequest<BaseMediatrResponse<GetDiscussionHistoriesForQualificationQueryResponse>>
{
    public string QualificationReference { get; set; } = string.Empty;
}
