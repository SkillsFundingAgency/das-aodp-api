using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using ChangedQualification = Entities.Qualification.ChangedQualification;

public interface IQualificationsRepository
{
    Task AddQualificationDiscussionHistory(Entities.Qualification.QualificationDiscussionHistory qualificationDiscussionHistory, string qualificationReference);

    Task<ProcessStatus> UpdateQualificationStatus(string qualificationReference, Guid processStatusId, int? version);

    Task<List<ChangedQualification>> GetChangedQualificationsAsync();

    Task<List<ProcessStatus>> GetProcessingStatuses();

    Task<IEnumerable<ChangedQualificationExport>> GetChangedQualificationsExport();

    Task<Entities.Qualification.Qualification> GetByIdAsync(string qualificationReference);

    Task<List<SearchedQualification>> GetSearchedQualificationByNameAsync(string name);
    Task<ProcessStatus> GetProcessStatusOrThrow(Guid processStatusId);

    Task<BulkUpdateQualificationStatusWithHistoryResult> BulkUpdateQualificationStatusWithHistoryAsync(
        IReadOnlyCollection<Guid> qualificationIds,
        Guid processStatusId,
        string userDisplayName,
        string? comment,
        CancellationToken cancellationToken = default);

    Task<QualificationVersions?> GetQualificationVersionByQanAsync(string qualificationReference, int? version, CancellationToken cancellationToken);
}