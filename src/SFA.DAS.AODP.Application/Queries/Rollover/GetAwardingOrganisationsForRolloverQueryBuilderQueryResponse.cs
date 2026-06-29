using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover;

public class GetAwardingOrganisationsForRolloverQueryBuilderQueryResponse
{
    public IEnumerable<RolloverQueryBuilderAwardingOrganisation> AwardingOrganisations { get; set; } = [];
}
