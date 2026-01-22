using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Util;
using SFA.DAS.AODP.Data.Context;

namespace SFA.DAS.AODP.Data.Search
{
    public class QualificationsIndexBuilder : IIndexBuilder
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IDirectoryFactory _directoryFactory;

        public QualificationsIndexBuilder(
            IApplicationDbContext applicationDbContext,
            IDirectoryFactory directoryFactory)
        {
            _applicationDbContext = applicationDbContext;
            _directoryFactory = directoryFactory;
        }

        // This method builds the Lucene index for qualifications
        // We need to get all the qualifications from the database
        // and index them using different analyzers for phrase, term, and n-gram searches
        public void Build()
        {
            var standardAnalyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            var pipeAnalyzer = new PipeAnalyzer();
            var ngramAnalyzer = new EdgeNGramAnalyzer();
            var fieldAnalyzers = new J2N.Collections.Generic.Dictionary<string, Analyzer>
            {
                //phrase
                {SearchableQualification.QualificationNamePhrase, pipeAnalyzer},
                //{SearchableQualification.QanPhrase, pipeAnalyzer},

                //term
                {SearchableQualification.QualificationNameTerm, standardAnalyzer},
                //{SearchableQualification.QanTerm, standardAnalyzer},

                //ngram
                {SearchableQualification.QualificationNameNGram, ngramAnalyzer}
            };
            var perFieldAnalyzerWrapper = new PerFieldAnalyzerWrapper(standardAnalyzer, fieldAnalyzers);

            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, perFieldAnalyzerWrapper);
            var directory = _directoryFactory.GetDirectory();

            // Clear existing index and build a new one
            using (var writer = new IndexWriter(directory, config))
            {
                writer.DeleteAll();
                writer.Commit();

                // Index each qualification from the database
                foreach (var qualification in _applicationDbContext.QualificationFundingStatus
                    .OrderBy(q => q.FundingStatus)
                    .ThenBy(q => q.AwardingOrganisationName)
                    .ThenBy(q => q.QualificationName))
                {
                    var doc = new Lucene.Net.Documents.Document();
                    var searchable = new SearchableQualification(qualification);

                    // Add all indexable fields to the document
                    foreach (var indexableField in searchable.GetFields())
                    {
                        doc.Add(indexableField);
                    }

                    writer.AddDocument(doc);
                }
                writer.Commit();
            }
        }
    }
}