using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Rollover
{
    public class GetRolloverWorkflowCandidatesQuery : IRequest<BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>>
    {
    }
}