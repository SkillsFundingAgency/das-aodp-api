using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Search;
using Entities = SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification;

public class GetMatchingQualificationsQueryHandlerTests
{
    private readonly Mock<IQualificationsSearchService> _mockSearchService;
    private readonly Mock<IQualificationsRepository> _mockRepository;
    private readonly Mock<ILogger<GetMatchingQualificationsQueryHandler>> _mockLogger ;
    private readonly GetMatchingQualificationsQueryHandler _handler;

    public GetMatchingQualificationsQueryHandlerTests()
    {
        _mockSearchService = new Mock<IQualificationsSearchService>(MockBehavior.Strict);
        _mockRepository = new Mock<IQualificationsRepository>(MockBehavior.Strict);
        _mockLogger = new Mock<ILogger<GetMatchingQualificationsQueryHandler>>();
        _handler = new GetMatchingQualificationsQueryHandler(_mockSearchService.Object, _mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReferenceSearch_RepositoryReturnsQualification_ReturnsSingleMappedItem()
    {
        // Arrange
        var input = "12/3456/78";
        var expectedId = "12345678";
        var qual = new Entities.Qualification
        {
            Id = Guid.NewGuid(),
            Qan = expectedId,
            QualificationName = "Test Qualification"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(expectedId))
            .ReturnsAsync(qual);

        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new Exception("Should not be called"));

        // Act
        var response = await _handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Value);
        Assert.Single(response.Value.Qualifications);
        var item = response.Value.Qualifications.First();
        Assert.Equal(qual.Id, item.Id);
        Assert.Equal(qual.Qan, item.Qan);
        Assert.Equal(qual.QualificationName, item.QualificationName);

        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task Handle_ReferenceSearch_LongerThan8_TakesRightMost8Digits()
    {
        // Arrange
        var input = "123/4567/89012"; 
        var expectedId = "56789012";
        var qual = new Entities.Qualification
        {
            Id = Guid.NewGuid(),
            Qan = expectedId,
            QualificationName = "Truncated Qualification"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(expectedId)).ReturnsAsync(qual);
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new Exception("Should not be called"));

        // Act
        var response = await _handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Single(response.Value.Qualifications);
        Assert.Equal(expectedId, response.Value.Qualifications.First().Qan);

        _mockRepository.Verify(r => r.GetByIdAsync(expectedId), Times.Once);
    }

    [Fact]
    public async Task Handle_ReferenceSearch_RepositoryReturnsNull_ReturnsEmptyList()
    {
        // Arrange
        var input = "12345678";
        var expectedId = "12345678";

        _mockRepository.Setup(r => r.GetByIdAsync(expectedId)).ReturnsAsync((Entities.Qualification?)null!);
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new Exception("Should not be called"));

        // Act
        var response = await _handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Value);
        Assert.Empty(response.Value.Qualifications);

        _mockRepository.Verify(r => r.GetByIdAsync(expectedId), Times.Once);
    }

    [Fact]
    public async Task Handle_NonReferenceSearch_CallsSearchServiceAndReturnsMappedItems()
    {
        // Arrange
        var input = "mathematics";
        var qual = new Entities.SearchedQualification
        {
            Id = Guid.NewGuid(),
            Qan = "87654321",
            QualificationName = "Maths"
        };

        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(input.Trim(), It.IsAny<CancellationToken>()))
                          .Returns(new[] { qual });

        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                       .Throws(new Exception("Should not be called"));

        // Act
        var response = await _handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.True(response.Success);
        Assert.Single(response.Value.Qualifications);
        var item = response.Value.Qualifications.First();
        Assert.Equal(qual.Id, item.Id);
        Assert.Equal(qual.Qan, item.Qan);
        Assert.Equal(qual.QualificationName, item.QualificationName);

        _mockSearchService.Verify(s => s.SearchQualificationsByKeywordAsync(input, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonReferenceSearch_SearchServiceThrowsRecordWithNameNotFoundException_ReturnsNoMatchesErrorCode()
    {
        // Arrange
        var input = "no-match-term";
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new RecordWithNameNotFoundException("not found"));

        // Repository should not be called in this branch but set strict to ensure.
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                       .Throws(new Exception("Should not be called"));

        // Act
        var response = await _handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("NO_MATCHES", response.ErrorCode);
        Assert.Contains(input, response.ErrorMessage);
    }

    [Fact]
    public async Task Handle_ReferenceSearch_RepositoryThrowsGenericException_ReturnsFailureWithMessageAndInnerException()
    {
        // Arrange
        var input = "12345678";
        var ex = new Exception("boom");
        _mockRepository.Setup(r => r.GetByIdAsync("12345678")).ThrowsAsync(ex);

        // Ensure search service is not called
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(new Exception("Should not be called"));

        // Act
        var response = await _handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("boom", response.ErrorMessage);
        Assert.NotNull(response.InnerException);
        Assert.Same(ex, response.InnerException);
    }

    [Fact]
    public async Task Handle_NonReferenceSearch_SearchServiceThrowsGenericException_ReturnsFailureWithMessageAndInnerException()
    {
        // Arrange
        var input = "biology";
        var ex = new Exception("boom2");
        _mockSearchService.Setup(s => s.SearchQualificationsByKeywordAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                          .Throws(ex);

        // Repository should not be called
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                       .Throws(new Exception("Should not be called"));

        // Act
        var response = await _handler.Handle(new GetMatchingQualificationsQuery { SearchTerm = input }, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("boom2", response.ErrorMessage);
        Assert.NotNull(response.InnerException);
        Assert.Same(ex, response.InnerException);
    }
}
