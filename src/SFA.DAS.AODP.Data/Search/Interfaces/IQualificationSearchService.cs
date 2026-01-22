using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Search
{
    public interface IQualificationsSearchService
    {
        IEnumerable<SearchedQualification> SearchQualificationsByKeywordAsync(string input, CancellationToken ct = default);
    }
}