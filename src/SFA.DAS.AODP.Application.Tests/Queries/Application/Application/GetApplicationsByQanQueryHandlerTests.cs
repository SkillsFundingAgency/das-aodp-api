using Moq;
using SFA.DAS.AODP.Application.Queries.Application.Application;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Application.Application;

public class GetApplicationsByQanQueryHandlerTests
{
    private readonly Mock<IApplicationRepository> _repositoryMock;
    private readonly GetApplicationsByQanQueryHandler _handler;

    public GetApplicationsByQanQueryHandlerTests()
    {
        _repositoryMock = new Mock<IApplicationRepository>(MockBehavior.Strict);
        _handler = new GetApplicationsByQanQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsApplications_ReturnsMappedResponse()
    {
        // Arrange
        var qan = "QAN123";
        var applicationId1 = Guid.NewGuid();
        var applicationId2 = Guid.NewGuid();
        var applicationReviewId1 = Guid.NewGuid();
        var applicationReviewId2 = Guid.NewGuid();
        var app1 = new SFA.DAS.AODP.Data.Entities.Application.Application
        {
            Id = applicationId1,
            Name = "App1",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            SubmittedAt = DateTime.UtcNow.AddDays(-1),
            ApplicationReview = new Data.Entities.Application.ApplicationReview
            { 
                Id = applicationReviewId1,
                ApplicationId = applicationId1,
            }
        };
        var app2 = new SFA.DAS.AODP.Data.Entities.Application.Application
        {
            Id = applicationId2,
            Name = "App2",
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            SubmittedAt = null,
            ApplicationReview = new Data.Entities.Application.ApplicationReview
            {
                Id = applicationReviewId2,
                ApplicationId = applicationId2,
            }
        };

        var list = new List<SFA.DAS.AODP.Data.Entities.Application.Application> { app1, app2 };

        _repositoryMock.Setup(r => r.GetByQan(qan)).ReturnsAsync(list);

        var query = new GetApplicationsByQanQuery(qan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByQan(qan), Times.Once);
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Applications.Count);

        Assert.Contains(result.Value.Applications, a => a.Id == app1.Id && a.Name == app1.Name && a.CreatedDate == app1.CreatedAt && a.SubmittedDate == app1.SubmittedAt && a.ApplicationReviewId == app1.ApplicationReview.Id);
        Assert.Contains(result.Value.Applications, a => a.Id == app2.Id && a.Name == app2.Name && a.CreatedDate == app2.CreatedAt && a.SubmittedDate == app2.SubmittedAt && a.ApplicationReviewId == app2.ApplicationReview.Id);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsEmptyList_ReturnsEmptyApplicationsAndSuccessTrue()
    {
        // Arrange
        var qan = "EMPTYQAN";
        _repositoryMock.Setup(r => r.GetByQan(qan)).ReturnsAsync(new List<SFA.DAS.AODP.Data.Entities.Application.Application>());

        var query = new GetApplicationsByQanQuery(qan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByQan(qan), Times.Once);
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Applications);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ReturnsFailureWithErrorMessage()
    {
        // Arrange
        var qan = "ERRQAN";
        var ex = new Exception("repository failure");
        _repositoryMock.Setup(r => r.GetByQan(qan)).ThrowsAsync(ex);

        var query = new GetApplicationsByQanQuery(qan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetByQan(qan), Times.Once);
        Assert.False(result.Success);
        Assert.Equal(ex.Message, result.ErrorMessage);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Applications);
    }
}
