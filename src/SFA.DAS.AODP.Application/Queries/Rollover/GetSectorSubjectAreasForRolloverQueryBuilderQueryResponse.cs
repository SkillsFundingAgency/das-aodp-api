using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetSectorSubjectAreasForRolloverQueryBuilderQueryResponse
{
    public IEnumerable<RolloverQueryBuilderSectorSubjectArea> SectorSubjectAreas { get; set; } = [];
}
