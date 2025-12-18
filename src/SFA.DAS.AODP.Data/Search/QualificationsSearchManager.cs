using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Index;
using Lucene.Net.Search;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Search
{
    public class QualificationsSearchManager : ISearchManager
    {
        private readonly IDirectoryFactory _directoryFactory;

        public QualificationsSearchManager(IDirectoryFactory directoryFactory)
        {
            _directoryFactory = directoryFactory;
        }

        public QualificationSearchResultsList Query(string searchTerm)
        {
            searchTerm = searchTerm.ToLowerInvariant().Trim();

            // Generate ngrams from the search term
            var ngramSearchTerms = TokenizeQuery(new EdgeNGramAnalyzer(), searchTerm);

            var boolQuery = new BooleanQuery();

            // This adds phrase queries to the bool query for exact matching
            boolQuery.Add(new PhraseQuery { new Term(SearchableQualification.QualificationNamePhrase, searchTerm) }, Occur.SHOULD);

            // This loop adds term queries to the bool query for individual term matching
            foreach (var term in searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                // Add term query for qualification name
                boolQuery.Add(new TermQuery(new Term(SearchableQualification.QualificationNameTerm, term)), Occur.SHOULD);
            }

            // this loop adds ngram terms to the query for partial matching
            foreach (var term in ngramSearchTerms)
            {
                // Add ngram term query for qualification name
                boolQuery.Add(new TermQuery(new Term(SearchableQualification.QualificationNameNGram, term)), Occur.SHOULD);
            }

            // Execute the search
            var directory = _directoryFactory.GetDirectory();

            // Use DirectoryReader to read the index
            var reader = DirectoryReader.Open(directory);

            // Create an IndexSearcher
            var searcher = new IndexSearcher(reader);

            // Search for the top 1000 documents matching the query
            var topDocs = searcher.Search(boolQuery, 1000);

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

            // Return the search results
            return new QualificationSearchResultsList
            {
                TotalCount = topDocs.TotalHits,
                Qualifications = results
            };
        }

        // This method tokenizes the input query using the provided analyzer
        private IEnumerable<string> TokenizeQuery(Analyzer analyzer, string query)
        {
            var result = new List<string>();

            // Use the analyzer to create a token stream
            using TokenStream tokenStream = analyzer.GetTokenStream(null, new StringReader(query));
            tokenStream.Reset();
            // Iterate through the tokens and collect them
            while (tokenStream.IncrementToken())
            {
                // Get the term attribute and add it to the result list
                result.Add(tokenStream.GetAttribute<ICharTermAttribute>().ToString());
            }

            return result;
        }
    }
}