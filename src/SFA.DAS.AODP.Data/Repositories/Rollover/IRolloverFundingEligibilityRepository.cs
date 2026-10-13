using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverFundingEligibilityRepository
{
    Task<IReadOnlyCollection<RolloverFundingEligibility>> GetAsync(
        IReadOnlyCollection<FundingChangeKey> keys,
        CancellationToken cancellationToken);
}
