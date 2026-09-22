using SFA.DAS.AODP.Data.Entities.QaaQualification;

namespace SFA.DAS.AODP.Data.Repositories.QaaQualification;

public interface IQaaQualificationDownloadLogRepository
{
    Task<Guid> CreateAsync(QaaQualificationDownloadLog entity, CancellationToken ct);

    Task<IReadOnlyList<QaaQualificationDownloadLog>> ListAsync(
        int? take = null,
        CancellationToken ct = default);
}
