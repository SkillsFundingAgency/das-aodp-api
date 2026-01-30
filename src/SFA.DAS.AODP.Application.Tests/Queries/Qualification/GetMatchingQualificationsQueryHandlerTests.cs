using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Search;
using SFA.DAS.AODP.Models.Settings;
using Entities = SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification;
public class GetMatchingQualificationsQueryHandlerTests
{
    private readonly Mock<IQualificationsSearchService> _mockSearchService;
    private readonly Mock<IQualificationsRepository> _mockRepository;
    private readonly Mock<ILogger<GetMatchingQualificationsQueryHandler>> _mockLogger;

    public GetMatchingQualificationsQueryHandlerTests()
    {
        _mockSearchService = new Mock<IQualificationsSearchService>(MockBehavior.Strict);
        _mockRepository = new Mock<IQualificationsRepository>(MockBehavior.Strict);
        _mockLogger = new Mock<ILogger<GetMatchingQualificationsQueryHandler>>();
    }

    private GetMatchingQualificationsQueryHandler CreateHandler(bool fuzzyEnabled)
    {
        var options = Options.Create(new FuzzySearchSettings { Enabled = fuzzyEnabled, Edits = 1, MinTokenLength = 3 });
        return new GetMatchingQualificationsQueryHandler(_mockSearchService.Object, _mockRepository.Object, options, _mockLogger.Object);
    }

    private static Entities.Qualification BuildQualification(string qan, Guid id, Guid statusId)
    {
        return new Entities.Qualification
        {
            Id = id,
            Qan = qan,
            QualificationName = "Name " + qan,
            QualificationVersions = new List<Entities.QualificationVersions>
            {
                new Entities.QualificationVersions { Version = 1, ProcessStatusId = statusId }
            }
        };
    }

    [Fact]
    public async Task Handle_ReferenceSearch_Found_ReturnsSingleMappedItem()
    {
        // Arrange
        var handler = CreateHandler(fuzzyEnabled: false);

        var input = "12/3456/78";
        var normalized = "12345678";
        var qualId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var qual = BuildQualification(normalized, qualId, statusId);

        _mockRepository.Setup(r => r.GetByIdAsync(normalized)).ReturnsAsync(qual);

        // Ensure search service is not called
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new Exception("Search should not be invoked for reference search"));

        // Act
        var response = await handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Value);
        Assert.Single(response.Value.Qualifications);
        var item = response.Value.Qualifications.First();
        Assert.Equal(qualId, item.Id);
        Assert.Equal(normalized, item.Qan);
        Assert.Equal(qual.QualificationName, item.QualificationName);
        Assert.Equal(statusId, item.Status);

        _mockRepository.Verify(r => r.GetByIdAsync(normalized), Times.Once);
        _mockSearchService.Verify(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReferenceSearch_LongInput_TruncatesToRightMost8()
    {
        // Arrange
        var handler = CreateHandler(fuzzyEnabled: false);

        var input = "123/4567/89012"; // normalized -> "123456789012" -> right-most 8 = "56789012"
        var expected = "56789012";
        var qual = BuildQualification(expected, Guid.NewGuid(), Guid.Empty);

        _mockRepository.Setup(r => r.GetByIdAsync(expected)).ReturnsAsync(qual);
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new Exception("Should not be called"));

        // Act
        var response = await handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Single(response.Value.Qualifications);
        Assert.Equal(expected, response.Value.Qualifications.First().Qan);

        _mockRepository.Verify(r => r.GetByIdAsync(expected), Times.Once);
    }

    [Fact]
    public async Task Handle_ReferenceSearch_RepositoryReturnsNull_ReturnsEmptyList()
    {
        // Arrange
        var handler = CreateHandler(fuzzyEnabled: false);

        var input = "12345678";
        _mockRepository.Setup(r => r.GetByIdAsync("12345678")).ReturnsAsync((Entities.Qualification?)null);
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new Exception("Should not be called"));

        // Act
        var response = await handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Value);
        Assert.Empty(response.Value.Qualifications);

        _mockRepository.Verify(r => r.GetByIdAsync("12345678"), Times.Once);
    }

    [Fact]
    public async Task Handle_NonReferenceSearch_FuzzyDisabled_UsesRepositoryAndPaginates()
    {
        // Arrange
        var handler = CreateHandler(fuzzyEnabled: false);

        var input = "mathematics";
        var results = new List<SearchedQualification>
        {
            new SearchedQualification { Id = Guid.NewGuid(), Qan = "Q1", QualificationName = "First" },
            new SearchedQualification { Id = Guid.NewGuid(), Qan = "Q2", QualificationName = "Second" },
            new SearchedQualification { Id = Guid.NewGuid(), Qan = "Q3", QualificationName = "Third" }
        };

        _mockRepository.Setup(r => r.GetSearchedQualificationByNameAsync(input.Trim()))
                       .ReturnsAsync(results);
        // search service should not be invoked
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new Exception("Search should not be invoked when fuzzy disabled"));

        var request = new GetMatchingQualificationsQuery { SearchTerm = input, Skip = 1, Take = 1 };

        // Act
        var response = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(3, response.Value.TotalRecords);
        Assert.Single(response.Value.Qualifications);
        Assert.Equal("Second", response.Value.Qualifications.Single().QualificationName);

        _mockRepository.Verify(r => r.GetSearchedQualificationByNameAsync(input.Trim()), Times.Once);
        _mockSearchService.Verify(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NonReferenceSearch_FuzzyEnabled_UsesSearchService()
    {
        // Arrange
        var handler = CreateHandler(fuzzyEnabled: true);

        var input = "fuzzy";
        var results = new List<SearchedQualification>
        {
            new SearchedQualification { Id = Guid.NewGuid(), Qan = "QF", QualificationName = "Fuzzy" }
        };

        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(input.Trim(), It.IsAny<CancellationToken>()))
                          .Returns(results);

        _mockRepository.Setup(r => r.GetSearchedQualificationByNameAsync(It.IsAny<string>()))
                       .Throws(new Exception("Repo should not be called when fuzzy enabled"));

        var response = await handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(1, response.Value.TotalRecords);
        Assert.Single(response.Value.Qualifications);
        Assert.Equal("Fuzzy", response.Value.Qualifications.Single().QualificationName);

        _mockSearchService.Verify(s => s.SearchQualificationsByKeywordAsync(input.Trim(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SearchService_Throws_RecordWithNameNotFoundException_ReturnsNoMatches()
    {
        // Arrange
        var handler = CreateHandler(fuzzyEnabled: true);

        var input = "no-match";
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new RecordWithNameNotFoundException("no matches"));

        // Act
        var response = await handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("NO_MATCHES", response.ErrorCode);
        Assert.Contains(input, response.ErrorMessage);
        Assert.NotNull(response.Value);
        Assert.Empty(response.Value.Qualifications);

        _mockSearchService.Verify(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_SearchService_Throws_OperationCanceledException_ReturnsTimeoutMessage()
    {
        // Arrange
        var handler = CreateHandler(fuzzyEnabled: true);

        var input = "timeout";
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new OperationCanceledException());

        // Act
        var response = await handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("The search operation timed out. Please try again later.", response.ErrorMessage);
        Assert.NotNull(response.Value);
        Assert.Empty(response.Value.Qualifications);

        _mockSearchService.Verify(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReferenceSearch_RepositoryThrows_GenericException_ReturnsFailureWithInnerException()
    {
        // Arrange
        var handler = CreateHandler(fuzzyEnabled: false);

        var input = "12345678";
        var ex = new Exception("boom");
        _mockRepository.Setup(r => r.GetByIdAsync("12345678")).ThrowsAsync(ex);
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new Exception("Should not be called"));

        // Act
        var response = await handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("boom", response.ErrorMessage);
        Assert.NotNull(response.InnerException);
        Assert.Same(ex, response.InnerException);

        _mockRepository.Verify(r => r.GetByIdAsync("12345678"), Times.Once);
    }

    [Fact]
    public async Task Handle_NonReferenceSearch_SearchServiceThrows_GenericException_ReturnsFailureWithInnerException()
    {
        // Arrange
        var handler = CreateHandler(fuzzyEnabled: true);

        var input = "biology";
        var ex = new Exception("boom2");
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(ex);

        // Act
        var response = await handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("boom2", response.ErrorMessage);
        Assert.NotNull(response.InnerException);
        Assert.Same(ex, response.InnerException);

        _mockSearchService.Verify(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
