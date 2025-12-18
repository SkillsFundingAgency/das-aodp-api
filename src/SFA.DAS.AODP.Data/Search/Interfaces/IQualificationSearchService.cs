using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Search
{
    public interface IQualificationsSearchService
    {
        IEnumerable<Qualification> SearchQualificationsByKeywordAsync(string input, int take = 25, CancellationToken ct = default);
    }
}