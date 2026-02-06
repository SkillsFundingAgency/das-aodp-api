using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.AODP.Data.Search;
using SFA.DAS.AODP.Models.Qualifications;
using SFA.DAS.AODP.Models.Settings;

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
            // phrase used for exact phrase matching
            new StringField(SearchableQualification.QualificationNamePhrase, phrase.ToLowerInvariant(), Field.Store.YES),

            // term field tokenized for term matching
            new TextField(SearchableQualification.QualificationNameTerm, termField.ToLowerInvariant(), Field.Store.YES),

            // ngram field stores prefixes / ngrams for partial matching
            new TextField(SearchableQualification.QualificationNameNGram, ngramField.ToLowerInvariant(), Field.Store.YES),

            // fields required by QualificationSearchResult constructor
            new StringField(nameof(QualificationSearchResult.Id), Guid.NewGuid().ToString(), Field.Store.YES),
            new StringField(nameof(QualificationSearchResult.QualificationName), phrase, Field.Store.YES),
            new StringField(nameof(QualificationSearchResult.Qan), "QAN-123", Field.Store.YES),
            new StringField(nameof(QualificationSearchResult.Status), Guid.NewGuid().ToString(), Field.Store.YES)
        };

        _writer.AddDocument(doc);
        _writer.Commit();
    }

    [Fact]
    public void Constructor_AllowsNullOptions()
    {
        // Arrange & Act
        var mgr = new QualificationsSearchManager(_directoryFactoryMock.Object, null);

        // Assert
        Assert.NotNull(mgr);
        Assert.IsAssignableFrom<ISearchManager>(mgr);
    }

    [Fact]
    public void Query_Returns_Result_For_PhraseMatch()
    {
        // Arrange
        var phrase = "My Qualification";
        AddDocument(phrase, "my qualification", "myq");

        var mgr = new QualificationsSearchManager(_directoryFactoryMock.Object, Options.Create(new FuzzySearchSettings { MinTokenLength = 0, Edits = 0 }));

        // Act
        var results = mgr.Query(phrase);

        // Assert
        Assert.Equal(1, results.TotalCount);
        Assert.Single(results.Qualifications);
        Assert.Equal(phrase, results.Qualifications.First().QualificationName);
    }

    [Fact]
    public void Query_Returns_Result_For_NGramMatch()
    {
        // Arrange
        AddDocument("Electric Appliance", "electric appliance", "elec");

        var mgr = new QualificationsSearchManager(_directoryFactoryMock.Object, Options.Create(new FuzzySearchSettings { MinTokenLength = 0, Edits = 0 }));

        // Act
        var results = mgr.Query("elec");

        // Assert
        Assert.Equal(1, results.TotalCount);
        Assert.Single(results.Qualifications);
        Assert.Equal("Electric Appliance", results.Qualifications.First().QualificationName);
    }

    [Fact]
    public void Query_Uses_FuzzyFallback_When_NoExactResults_And_FuzzySettingsAllow()
    {
        // Arrange
        AddDocument("Electrician", "electrician", "elec");

        var fuzzyOptions = Options.Create(new FuzzySearchSettings { MinTokenLength = 3, Edits = 2 });
        var mgr = new QualificationsSearchManager(_directoryFactoryMock.Object, fuzzyOptions);

        // Act 
        var results = mgr.Query("electrican");

        // Assert
        Assert.True(results.TotalCount > 0);
        Assert.Single(results.Qualifications);
        Assert.Equal("Electrician", results.Qualifications.First().QualificationName);
    }

    [Fact]
    public void Query_Returns_Empty_When_NoMatch_And_Fuzzy_NotApplied()
    {
        // Arrange
        AddDocument("Something Else", "something else", "som");

        var fuzzyOptions = Options.Create(new FuzzySearchSettings { MinTokenLength = 100, Edits = 2 });
        var mgr = new QualificationsSearchManager(_directoryFactoryMock.Object, fuzzyOptions);

        // Act
        var results = mgr.Query("nomatch");

        // Assert
        Assert.Equal(0, results.TotalCount);
        Assert.Empty(results.Qualifications);
    }
}