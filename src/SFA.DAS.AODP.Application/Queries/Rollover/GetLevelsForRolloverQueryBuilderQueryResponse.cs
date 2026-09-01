using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetLevelsForRolloverQueryBuilderQueryResponse
{
    public IEnumerable<RolloverQueryBuilderLevel> Levels { get; set; } = [];
}
