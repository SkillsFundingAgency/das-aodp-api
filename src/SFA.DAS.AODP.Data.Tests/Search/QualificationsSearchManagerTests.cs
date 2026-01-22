using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Moq;
using SFA.DAS.AODP.Data.Search;

namespace SFA.DAS.AODP.Data.UnitTests.Search;
public class QualificationsSearchManagerTests : IDisposable
{
    private readonly RAMDirectory _directory;
    private readonly IndexWriter _writer;
    private readonly Mock<IDirectoryFactory> _directoryFactoryMock;

    public QualificationsSearchManagerTests()
    {
        _directory = new RAMDirectory();
        var analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(LuceneVersion.LUCENE_48);
        var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
        _writer = new IndexWriter(_directory, config);

        _directoryFactoryMock = new Mock<IDirectoryFactory>();
        _directoryFactoryMock.Setup(f => f.GetDirectory()).Returns(_directory);
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _directory?.Dispose();
    }

    private void AddDocument(string phrase, string termField, string ngramField)
    {
        var doc = new Document
            {
                // phrase should be indexed as an exact term for phrase matching in this simple setup
                new StringField(SearchableQualification.QualificationNamePhrase, phrase.ToLowerInvariant(), Field.Store.YES),

                // term field should be tokenized so term queries for words match
                new TextField(SearchableQualification.QualificationNameTerm, termField.ToLowerInvariant(), Field.Store.YES),

                // ngram field contains small substrings/prefixes so ngram term queries match
                new TextField(SearchableQualification.QualificationNameNGram, ngramField.ToLowerInvariant(), Field.Store.YES)
            };

        _writer.AddDocument(doc);
        _writer.Flush(triggerMerge: false, applyAllDeletes: false);
    }

    [Fact]
    public void Constructor_AllowsNullOptions()
    {
        // Arrange & Act
        var mgr = new QualificationsSearchManager(_directoryFactoryMock.Object, null);

        // Assert - ensure object created and is ISearchManager
        Assert.NotNull(mgr);
        Assert.IsAssignableFrom<ISearchManager>(mgr);
    }
}