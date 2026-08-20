using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.QaaQualification;

public interface IQaaQualificationFundingsRepository
{
    Task CreateAsync(QaaQualificationFunding funding, CancellationToken cancellationToken);

    Task<List<QaaQualificationFunding>> GetByQaaQualificationIdAsync(
        Guid qaaQualificationId,
        CancellationToken cancellationToken);

    Task<List<QaaQualificationFunding>> GetRolloverQaaQualificationFundingsAsync(
        List<SourceQualificationFundingKey> candidates,
        CancellationToken cancellationToken);
}
