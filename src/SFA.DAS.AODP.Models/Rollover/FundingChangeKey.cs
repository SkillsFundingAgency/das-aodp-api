using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Rollover;

[ExcludeFromCodeCoverage]
public record FundingChangeKey(
    string SourceType,
    Guid SourceQualificationId,
    Guid FundingOfferId,
    string? AcademicYear = null);

[ExcludeFromCodeCoverage]
public sealed record FundingChangeSet(IReadOnlyCollection<FundingChangeKey> Keys)
{
    public static FundingChangeSet Create(IEnumerable<FundingChangeKey> keys)
    {
        return new FundingChangeSet(keys.Distinct().ToList());
    }
}
