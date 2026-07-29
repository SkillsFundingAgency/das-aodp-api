using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverFundingUpdateRepository
{
    Task<List<RolloverFundingUpdate>> GetFundingUpdatesAsync(
        List<SourceQualificationFundingKey> candidates,
        CancellationToken cancellationToken);
}
