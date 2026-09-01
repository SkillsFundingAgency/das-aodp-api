using MediatR;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetSectorSubjectAreasForRolloverQueryBuilderQuery(RolloverQueryBuilderSectorSubjectAreaRequest filters)
    : IRequest<BaseMediatrResponse<GetSectorSubjectAreasForRolloverQueryBuilderQueryResponse>>
{
    public RolloverQueryBuilderSectorSubjectAreaRequest Filters { get; } = filters;
}
