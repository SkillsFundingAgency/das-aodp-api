using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Rollover;

public class GetRolloverWorkflowCandidatesCountQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRolloverRepository> _repositoryMock;
    private readonly GetRolloverWorkflowCandidatesCountQueryHandler _handler;

    public GetRolloverWorkflowCandidatesCountQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _repositoryMock = _fixture.Freeze<Mock<IRolloverRepository>>();
        _handler = _fixture.Create<GetRolloverWorkflowCandidatesCountQueryHandler>();
    }

    [Fact]
    public async Task Handle_ReturnsData_WhenRepositoryReturnsData()
    {
        // Arrange
        var query = new GetRolloverWorkflowCandidatesCountQuery();
        var repoResult = new RolloverWorkflowCandidatesCountResult(2);

        _repositoryMock.Setup(r => r.GetRolloverWorkflowCandidatesCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(repoResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetRolloverWorkflowCandidatesCountAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.TotalRecords);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenRepositoryThrows()
    {
        // Arrange
        var query = new GetRolloverWorkflowCandidatesCountQuery();
        var exceptionMessage = "boom";
        _repositoryMock.Setup(r => r.GetRolloverWorkflowCandidatesCountAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.GetRolloverWorkflowCandidatesCountAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.False(result.Success);
        Assert.Equal(exceptionMessage, result.ErrorMessage);
    }
}
