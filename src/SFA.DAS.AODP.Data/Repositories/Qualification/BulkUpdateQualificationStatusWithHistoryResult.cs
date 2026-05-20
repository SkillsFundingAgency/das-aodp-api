using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

public class BulkUpdateQualificationStatusWithHistoryResult
{
    public required ProcessStatus Status { get; init; }

    public required IReadOnlyList<(Guid QualificationVersionId, string Qan)> Succeeded { get; init; }

    public required IReadOnlyList<Guid> MissingIds { get; init; }

    public required IReadOnlyList<(Guid QualificationId, string Qan, string Message)> StatusUpdateFailed { get; init; }

    public required IReadOnlyList<(Guid QualificationId, string Qan, string Message)> HistoryFailed { get; init; }
}