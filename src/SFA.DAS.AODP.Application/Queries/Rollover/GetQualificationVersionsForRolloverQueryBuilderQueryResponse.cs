using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetQualificationVersionsForRolloverQueryBuilderQueryResponse
{
    public IEnumerable<RolloverQualificationVersion> QualificationVersions { get; set; } = [];
}
