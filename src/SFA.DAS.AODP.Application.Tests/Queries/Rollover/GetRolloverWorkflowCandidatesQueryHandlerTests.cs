using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Rollover;

public class GetRolloverWorkflowCandidatesQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRolloverRepository> _repositoryMock;
    private readonly GetRolloverWorkflowCandidatesQueryHandler _handler;

    public GetRolloverWorkflowCandidatesQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _repositoryMock = _fixture.Freeze<Mock<IRolloverRepository>>();
        _handler = _fixture.Create<GetRolloverWorkflowCandidatesQueryHandler>();
    }

    [Fact]
    public async Task Handle_ReturnsData_WhenRepositoryReturnsData()
    {
        // Arrange
        var query = new GetRolloverWorkflowCandidatesQuery(0, 10);
        var repoResult = new RolloverWorkflowCandidatesResult
        {
            Data = _fixture.CreateMany<RolloverWorkflowCandidate>(2).ToList(),
            Skip = 0,
            Take = 10,
            TotalRecords = 2
        };

        _repositoryMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(query.Skip, query.Take))
            .ReturnsAsync(repoResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllRolloverWorkflowCandidatesAsync(query.Skip, query.Take), Times.Once);
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Data.Count);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenRepositoryReturnsNull()
    {
        // Arrange
        var query = new GetRolloverWorkflowCandidatesQuery(0, 10);

        _repositoryMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(query.Skip, query.Take))
            .ReturnsAsync((RolloverWorkflowCandidatesResult?)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllRolloverWorkflowCandidatesAsync(query.Skip, query.Take), Times.Once);
        Assert.False(result.Success);
        Assert.Equal("No rollover workflow candidates found.", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenRepositoryThrows()
    {
        // Arrange
        var query = new GetRolloverWorkflowCandidatesQuery(0, 10);
        var exceptionMessage = "boom";
        _repositoryMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(query.Skip, query.Take))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetAllRolloverWorkflowCandidatesAsync(query.Skip, query.Take), Times.Once);
        Assert.False(result.Success);
        Assert.Equal(exceptionMessage, result.ErrorMessage);
    }
}
