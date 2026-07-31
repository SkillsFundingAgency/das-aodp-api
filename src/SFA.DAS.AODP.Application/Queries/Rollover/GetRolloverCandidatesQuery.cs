using MediatR;
using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Application.Queries.Rollover
{
    public class GetRolloverCandidatesQuery : IRequest<BaseMediatrResponse<GetRolloverCandidatesQueryResponse>>
    {
    }
}