using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Extensions.Options;
using SFA.DAS.AODP.Models.Qualifications;
using SFA.DAS.AODP.Models.Settings;

namespace SFA.DAS.AODP.Data.Search;

public class QualificationsSearchManager : ISearchManager
{
    private readonly IDirectoryFactory _directoryFactory;
    private readonly int _maxFuzzyEdits;
    private readonly int _minTokenLengthForFuzzy;

    private const int MaxResultsCap = 10000;

    public QualificationsSearchManager(IDirectoryFactory directoryFactory, IOptions<FuzzySearchSettings> fuzzySearchOptions)
    {
        _directoryFactory = directoryFactory;

        _maxFuzzyEdits = fuzzySearchOptions?.Value?.Edits ?? 0;
        _minTokenLengthForFuzzy = fuzzySearchOptions?.Value?.MinTokenLength ?? 0;
    }

    public QualificationSearchResultsList Query(string searchTerm)
    {
        searchTerm = searchTerm.ToLowerInvariant().Trim();

        var boolQuery = new BooleanQuery();

        // This adds phrase queries to the bool query for exact matching
        boolQuery.Add(new PhraseQuery { new Term(SearchableQualification.QualificationNamePhrase, searchTerm) }, Occur.SHOULD);

        // This loop adds term queries to the bool query for individual term matching
        foreach (var term in searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            // Add term query for qualification name
            boolQuery.Add(new TermQuery(new Term(SearchableQualification.QualificationNameTerm, term)), Occur.SHOULD);
        }

        // Execute the search
        var directory = _directoryFactory.GetDirectory();

        // Use DirectoryReader to read the index
        var reader = DirectoryReader.Open(directory);

        // Create an IndexSearcher
        var searcher = new IndexSearcher(reader);

        // Search for the top documents matching the query
        var topDocs = searcher.Search(boolQuery, MaxResultsCap);

        // Process the search results
        var results = new List<QualificationSearchResult>();

        // Iterate through the matched documents
        foreach (var scoreDoc in topDocs.ScoreDocs)
        {
            // Retrieve the document and score the result
            var doc = searcher.Doc(scoreDoc.Doc);

            // Add to results
            results.Add(new QualificationSearchResult(doc, scoreDoc.Score));
        }

        // If no results, try a fuzzy fallback (handles early-character typos like 'elactri' vs 'electric')
        if (topDocs.TotalHits == 0)
        {
            var fuzzyBool = new BooleanQuery();

            // Fuzzy on term tokens (qualification name term field)
            foreach (var term in searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                // Skip very short tokens — they create false positives (and n-grams are handled separately above)
                if (term.Length < _minTokenLengthForFuzzy) continue;

                fuzzyBool.Add(new FuzzyQuery(new Term(SearchableQualification.QualificationNameTerm, term), _maxFuzzyEdits), Occur.SHOULD);
            }

            // NOTE: intentionally NOT applying fuzzy to ngram tokens because n-grams are short and will match widely.

            var fuzzyTopDocs = searcher.Search(fuzzyBool, MaxResultsCap);

            if (fuzzyTopDocs.TotalHits > 0)
            {
                results.Clear();
                foreach (var sd in fuzzyTopDocs.ScoreDocs)
                {
                    var d = searcher.Doc(sd.Doc);
                    results.Add(new QualificationSearchResult(d, sd.Score));
                }

                return new QualificationSearchResultsList
                {
                    TotalCount = fuzzyTopDocs.TotalHits,
                    Qualifications = results
                };
            }
        }

        return new QualificationSearchResultsList
        {
            TotalCount = topDocs.TotalHits,
            Qualifications = results
        };
    }
}