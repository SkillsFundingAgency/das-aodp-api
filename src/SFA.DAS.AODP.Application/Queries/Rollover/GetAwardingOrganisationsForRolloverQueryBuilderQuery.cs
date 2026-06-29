using MediatR;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetAwardingOrganisationsForRolloverQueryBuilderQuery(RolloverQueryBuilderRequest filters)
    : IRequest<BaseMediatrResponse<GetAwardingOrganisationsForRolloverQueryBuilderQueryResponse>>
{
    public RolloverQueryBuilderRequest Filters { get; } = filters;
}
