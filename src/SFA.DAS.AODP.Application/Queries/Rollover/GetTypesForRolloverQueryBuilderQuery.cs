using MediatR;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetTypesForRolloverQueryBuilderQuery(RolloverQueryBuilderTypesRequest filters)
    : IRequest<BaseMediatrResponse<GetTypesForRolloverQueryBuilderQueryResponse>>
{
    public RolloverQueryBuilderTypesRequest Filters { get; } = filters;
}
