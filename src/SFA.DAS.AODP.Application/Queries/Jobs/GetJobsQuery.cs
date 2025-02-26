using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Jobs
{
    public class GetJobsQuery : IRequest<BaseMediatrResponse<GetJobsQueryResponse>>
    {
    }

}
