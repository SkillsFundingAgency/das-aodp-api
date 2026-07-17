using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetTypesForRolloverQueryBuilderQueryResponse
{
    public IEnumerable<RolloverQueryBuilderType> Types { get; set; } = [];
}
