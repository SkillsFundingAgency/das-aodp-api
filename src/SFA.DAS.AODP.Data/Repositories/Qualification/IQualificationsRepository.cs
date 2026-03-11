using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using ChangedQualification = Entities.Qualification.ChangedQualification;

public interface IQualificationsRepository
{
    Task AddQualificationDiscussionHistory(Entities.Qualification.QualificationDiscussionHistory qualificationDiscussionHistory, string qualificationReference);
    Task<List<ChangedQualification>> GetChangedQualificationsAsync();
    Task<ProcessStatus> UpdateQualificationStatus(string qualificationReference, Guid processStatusId, int? version);
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

    public class BulkUpdateQualificationStatusWithHistoryResult
    {
        public required ProcessStatus Status { get; init; }

        public required IReadOnlyList<(Guid QualificationVersionId, string Qan)> Succeeded { get; init; }

        public required IReadOnlyList<Guid> MissingIds { get; init; }

        public required IReadOnlyList<(Guid QualificationId, string Qan, string Message)> StatusUpdateFailed { get; init; }

        public required IReadOnlyList<(Guid QualificationId, string Qan, string Message)> HistoryFailed { get; init; }
    }
}
