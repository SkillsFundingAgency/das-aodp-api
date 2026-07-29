using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IFundingChangeCoordinator
{
    Task<TResult> ExecuteAsync<TResult>(
        FundingChangeSet changeSet,
        Func<CancellationToken, Task<TResult>> applyFundingMutation,
        CancellationToken cancellationToken);
}
