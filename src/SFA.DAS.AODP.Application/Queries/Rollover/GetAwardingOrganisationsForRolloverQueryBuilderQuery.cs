using MediatR;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetAwardingOrganisationsForRolloverQueryBuilderQuery(RolloverQueryBuilderAwardingOrganisationsRequest filters)
    : IRequest<BaseMediatrResponse<GetAwardingOrganisationsForRolloverQueryBuilderQueryResponse>>
{
    public RolloverQueryBuilderAwardingOrganisationsRequest Filters { get; } = filters;
}
