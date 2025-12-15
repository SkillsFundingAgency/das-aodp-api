using J2N.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
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
            IApplicationDbContext coursesDataContext,
            IDirectoryFactory directoryFactory)
        {
            _applicationDbContext = coursesDataContext;
            _directoryFactory = directoryFactory;
        }

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

            using (var writer = new IndexWriter(directory, config))
            {
                writer.DeleteAll();
                writer.Commit();

                foreach (var qualification in _applicationDbContext.Qualification)
                {
                    var doc = new Document();
                    var searchable = new SearchableQualification(qualification);

                    foreach (var indexableField in searchable.GetFields())
                    {
                        doc.Add(indexableField);
                    }

                    writer.AddDocument(doc);
                    writer.Commit();
                }
            }
        }
    }
}
