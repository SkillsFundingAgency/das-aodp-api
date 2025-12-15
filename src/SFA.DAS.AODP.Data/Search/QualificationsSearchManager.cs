using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;

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

            var ngramSearchTerms = TokenizeQuery(new EdgeNGramAnalyzer(), searchTerm);

            var boolQuery = new BooleanQuery();

            //phrase
            boolQuery.Add(new PhraseQuery { new Term(SearchableQualification.QualificationNamePhrase, searchTerm) }, Occur.SHOULD);
            //boolQuery.Add(new PhraseQuery { new Term(SearchableQualification.QanPhrase, searchTerm) }, Occur.SHOULD);

            //term
            foreach (var term in searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                boolQuery.Add(new TermQuery(new Term(SearchableQualification.QualificationNameTerm, term)), Occur.SHOULD);
                //boolQuery.Add(new TermQuery(new Term(SearchableQualification.QanTerm, term)), Occur.SHOULD);
            }

            //ngram
            foreach (var term in ngramSearchTerms)
            {
                boolQuery.Add(new TermQuery(new Term(SearchableQualification.QualificationNameNGram, term)), Occur.SHOULD);
            }

            var directory = _directoryFactory.GetDirectory();
            var reader = DirectoryReader.Open(directory);
            var searcher = new IndexSearcher(reader);

            var topDocs = searcher.Search(boolQuery, 1000);

            var results = new List<QualificationSearchResult>();
            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var doc = searcher.Doc(scoreDoc.Doc);
                results.Add(new QualificationSearchResult(doc, scoreDoc.Score));
            }

            return new QualificationSearchResultsList
            {
                TotalCount = topDocs.TotalHits,
                Qualifications = results
            };
        }

        private IEnumerable<string> TokenizeQuery(Analyzer analyzer, string query)
        {
            var result = new List<string>();

            using TokenStream tokenStream = analyzer.GetTokenStream(null, new StringReader(query));
            tokenStream.Reset();
            while (tokenStream.IncrementToken())
            {
                result.Add(tokenStream.GetAttribute<ICharTermAttribute>().ToString());
            }

            return result;
        }
    }
}
