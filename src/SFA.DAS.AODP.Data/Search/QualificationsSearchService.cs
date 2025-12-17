using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Search;
public class QualificationsSearchService : IQualificationsSearchService
{
    private readonly ISearchManager _searchManager;

    public QualificationsSearchService(ISearchManager searchManager)
    {
        _searchManager = searchManager;
    }

    public async Task<IEnumerable<Qualification>> SearchQualificationsByKeywordAsync(string keyword, int take = 25, CancellationToken ct = default)
    {
        var queryResult = _searchManager.Query(keyword);
        var results = queryResult.Qualifications
            .Select(q => new Qualification
            {
                Id = Guid.Parse(q.Id),
                QualificationName = q.QualificationName,
                Qan = q.Qan
            })
            .Take(take)
            .ToList();
        return await Task.FromResult((IReadOnlyList<Qualification>)results);
    }

}