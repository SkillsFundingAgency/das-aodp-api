using Azure;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Search;
public class QualificationsSearchService : IQualificationsSearchService
{
    private readonly ISearchManager _searchManager;

    public QualificationsSearchService(ISearchManager searchManager)
    {
        _searchManager = searchManager;
    }

    public IEnumerable<Qualification> SearchQualificationsByKeywordAsync(string searchTerm, int take = 25, CancellationToken ct = default)
    {

        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search Term cannot be empty.", nameof(searchTerm));


        ct.ThrowIfCancellationRequested();

        var queryResult = _searchManager.Query(searchTerm);

        var results = queryResult.Qualifications
            .Select(q =>
            {
                // TryParse avoids exceptions on bad GUIDs
                if (!Guid.TryParse(q.Id, out var id))
                    return null;

                return new Qualification
                {
                    Id = id,
                    QualificationName = q.QualificationName,
                    Qan = q.Qan
                };
            })
            .Where(x => x != null)
            .Take(take)
            .ToList()!;


        if (results.Count == 0)
        {
            throw new RecordWithNameNotFoundException($"No qualifications were found matching search term: {searchTerm}");
        }

        return results;

    }

}