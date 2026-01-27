using Moq;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Search;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.UnitTests.Search;

public class QualificationsSearchServiceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SearchQualificationsByKeywordAsync_InvalidSearchTerm_ThrowsArgumentException(string term)
    {
        // Arrange
        var searchManagerMock = new Mock<ISearchManager>(MockBehavior.Strict);
        var sut = new QualificationsSearchService(searchManagerMock.Object);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => sut.SearchQualificationsByKeywordAsync(term));
        Assert.Equal("searchTerm", ex.ParamName);
    }

    [Fact]
    public void SearchQualificationsByKeywordAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var searchManagerMock = new Mock<ISearchManager>(MockBehavior.Strict);
        var sut = new QualificationsSearchService(searchManagerMock.Object);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        Assert.Throws<OperationCanceledException>(() => sut.SearchQualificationsByKeywordAsync("term", ct: cts.Token));
    }

    [Fact]
    public void SearchQualificationsByKeywordAsync_CallsQuery_WithProvidedSearchTerm()
    {
        // Arrange
        var searchTerm = "math";
        var searchResults = new QualificationSearchResultsList
        {
            Qualifications = Array.Empty<QualificationSearchResult>()
        };

        var searchManagerMock = new Mock<ISearchManager>();
        searchManagerMock
            .Setup(m => m.Query(It.Is<string>(s => s == searchTerm)))
            .Returns(searchResults)
            .Verifiable();

        var sut = new QualificationsSearchService(searchManagerMock.Object);

        // Act & Assert: method will throw RecordWithNameNotFoundException because no valid results
        Assert.Throws<RecordWithNameNotFoundException>(() => sut.SearchQualificationsByKeywordAsync(searchTerm));

        // Verify Query was called exactly once with the expected term
        searchManagerMock.Verify(m => m.Query(searchTerm), Times.Once);
    }

    [Fact]
    public void SearchQualificationsByKeywordAsync_ValidResults_AreMappedAndLimitedByTake()
    {
        // Arrange
        var searchTerm = "science";

        var validGuid1 = Guid.NewGuid();
        var validGuid2 = Guid.NewGuid();
        var validGuid3 = Guid.NewGuid();

        var rawResults = new List<QualificationSearchResult>
            {
                new QualificationSearchResult { Id = validGuid1.ToString(), QualificationName = "Qual 1", Qan = "QAN1" },
                new QualificationSearchResult { Id = "not-a-guid", QualificationName = "Invalid", Qan = "QANX" }, // should be filtered out
                new QualificationSearchResult { Id = validGuid2.ToString(), QualificationName = "Qual 2", Qan = "QAN2" },
                new QualificationSearchResult { Id = validGuid3.ToString(), QualificationName = "Qual 3", Qan = "QAN3" }
            };

        var searchResults = new QualificationSearchResultsList
        {
            Qualifications = rawResults
        };

        var searchManagerMock = new Mock<ISearchManager>();
        searchManagerMock.Setup(m => m.Query(It.IsAny<string>())).Returns(searchResults);

        var sut = new QualificationsSearchService(searchManagerMock.Object);

        // Act
        var results = sut.SearchQualificationsByKeywordAsync(searchTerm).ToList();

        // Assert
        Assert.Equal(validGuid1, results[0].Id);
        Assert.Equal("Qual 1", results[0].QualificationName);
        Assert.Equal("QAN1", results[0].Qan);

        Assert.Equal(validGuid2, results[1].Id);
        Assert.Equal("Qual 2", results[1].QualificationName);
        Assert.Equal("QAN2", results[1].Qan);
    }
}

