using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Funding;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IFundingDomainEventDispatcher
{
    Task DispatchAsync(
        ApplicationDbContext context,
        IReadOnlyCollection<FundingDomainEvent> events,
        CancellationToken cancellationToken);
}
