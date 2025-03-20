using MediatR;

namespace SFA.DAS.AODP.Application.Queries.Qualifications
{
    public class GetProcessingStatusesQuery: IRequest<BaseMediatrResponse<GetProcessingStatusesQueryResponse>>
    {
    }
}
