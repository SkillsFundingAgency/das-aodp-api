using MediatR;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetLevelsForRolloverQueryBuilderQuery : IRequest<BaseMediatrResponse<GetLevelsForRolloverQueryBuilderQueryResponse>>
{
}
