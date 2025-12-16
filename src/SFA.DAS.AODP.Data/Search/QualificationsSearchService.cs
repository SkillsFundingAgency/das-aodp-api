using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Data.Search;
public class QualificationsSearchService : IQualificationsSearchService
{
    private readonly ISearchManager _searchManager;
    //private readonly IQualificationDetailsRepository _qualificationDetailsRepository;

    public QualificationsSearchService(ISearchManager searchManager, IQualificationDetailsRepository qualificationDetailsRepository)
    {
        _searchManager = searchManager;

    }

    public async Task<IReadOnlyList<QualificationSearchResult>> SearchQualificationsAsync(string input, int take = 25, CancellationToken ct = default)
    {
        var topDocs = _searchManager.Query(input);
        var results = topDocs.Qualifications
            .Select(q => new QualificationSearchResult
            {
                Id = q.Id,
                Score = q.Score
            })
            .Take(take)
            .ToList();
        return await Task.FromResult((IReadOnlyList<QualificationSearchResult>)results);
    }

    //public IEnumerable<QualificationSearchResultDto> SearchQualificationsByKeyword(string keyword)
    //{
    //    var queryResult = _searchManager.Query(keyword);

    //    return queryResult.Qualifications
    //        .Select(q => new QualificationSearchResultDto
    //        {
    //            Id = Guid.Parse(q.Id),
    //            Qan = q.Id,
    //            QualificationName = q
    //        });
    //}

    //private IEnumerable<QualificationSearchResultDto> SearchQualificationsByKeyword(string keyword)
    //{
    //    // Query the search index
    //    var queryResult = _searchManager.Query(keyword);

    //    // Join the results with the qualifications to get the matched qualifications
    //    var tempQualifications = qualifications
    //        .Join(queryResult.Qualifications,
    //            qualification => qualification.Id.ToString(),
    //            searchQualification => searchQualification.Id,
    //            (qualification, searchQualification) => new { qualification, searchQualification })
    //        .ToList();

    //    // Map the search scores back to the qualifications
    //    foreach (var tempQualification in tempQualifications)
    //    {
    //        //tempQualification.qualification.SearchScore = tempQualification.searchQualification.Score;
    //    }

    //    // Return the matched qualifications
    //    qualifications = tempQualifications
    //        .Select(t => t.qualification);

    //    return qualifications;
    //}

    //public IEnumerable<QualificationSearchResultDto> SearchQualificationsByKeyword(string keyword)
    //{
    //    // Query Lucene ONLY to get the IDs (Qans) and Scores.
    //    var queryResult = _searchManager.Query(keyword);
    //    var searchMatches = queryResult.Qualifications.ToList(); // List of { Id (string/Qan), Score (float) }

    //    if (!searchMatches.Any())
    //    {
    //        return Enumerable.Empty<QualificationSearchResultDto>();
    //    }

    //    // get the list of Qan strings from the Lucene results
    //    var matchingQans = searchMatches.Select(m => m.Id).ToList();

    //    // get these specific records from the DB using the QANs list
    //    var dbQualifications = _repository.GetByQans(matchingQans).ToList();


    //}

}