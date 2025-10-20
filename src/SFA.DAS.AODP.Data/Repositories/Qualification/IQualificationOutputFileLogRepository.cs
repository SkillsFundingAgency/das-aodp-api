using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface IQualificationOutputFileLogRepository
    {
        Task<Guid> CreateAsync(QualificationOutputFileLog entity, CancellationToken ct);

        Task<IReadOnlyList<QualificationOutputFileLog>> ListAsync(
            int? take = null,
            CancellationToken ct = default);
    }
}