using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobRunsQuery : IRequest<BaseMediatrResponse<GetJobRunsQueryResponse>>
    {
    }
}
