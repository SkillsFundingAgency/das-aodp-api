using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.FundingExtension;

public interface IFundingExtensionPersistenceRepository
{
    Task PersistAsync(
        IReadOnlyCollection<RolloverCandidates> candidates,
        IReadOnlyCollection<QualificationFundings> fundings,
        IReadOnlyCollection<QualificationDiscussionHistory> histories,
        CancellationToken cancellationToken);
}
