using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.FundingExtension;

public interface IFundingExtensionPersistenceRepository
{
    Task PersistAsync(
        IReadOnlyCollection<RolloverCandidates> candidates,
        IReadOnlyCollection<RolloverFundingUpdate> fundingUpdates,
        IReadOnlyCollection<QualificationDiscussionHistory> histories,
        IReadOnlyCollection<QaaQualificationDiscussionHistory> qaaHistories,
        CancellationToken cancellationToken);
}

