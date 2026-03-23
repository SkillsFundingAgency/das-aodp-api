using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using System.Reflection;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Rollover;

public class UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandlerTests
{
    private readonly Mock<IRolloverRepository> _repoMock;
    private readonly UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandler _handler;

    public UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandlerTests()
    {
        _repoMock = new Mock<IRolloverRepository>();
        _handler = new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_NoChecks_ReturnsSuccessAndDoesNotCallUpdate()
    {
        // Arrange
        _repoMock
            .Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _repoMock
            .Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _repoMock.Verify(r => r.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CandidatePassesAllChecks_UpdatesCandidateWithPassTrue()
    {
        // Arrange
        var id = Guid.NewGuid();

        var candidate = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", DateTime.UtcNow.AddDays(-1), null, DateTime.UtcNow.AddDays(-1));
        typeof(RolloverWorkflowCandidate)
            .GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(candidate, id);

        var p1 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = id,
            FundingStream = "FS",
            LatestFundingApprovalEndDate = null,
            ThresholdDate = DateTime.UtcNow.AddDays(-10),
            OperationalEndDate = null,
            OfferedInEngland = true,
            Glh = 10,
            Tqt = 20,
            IsOnDefundingList = false
        };

        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([p1]);

        _repoMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([candidate]);

        IEnumerable<RolloverWorkflowCandidate>? updatedArg = null;
        _repoMock.Setup(r => r.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()))
                 .Callback<IEnumerable<RolloverWorkflowCandidate>, CancellationToken>((c, ct) => updatedArg = c)
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _repoMock.Verify(r => r.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(updatedArg);
        var saved = updatedArg!.Single();
        Assert.True(saved.PassP1);
        Assert.Null(saved.P1FailureReason);
    }

    [Fact]
    public async Task Handle_CandidateFailsAllChecks_UpdatesCandidateWithFailAndConcatenatedReasons()
    {
        // Arrange
        var id = Guid.NewGuid();

        var candidate = RolloverWorkflowCandidate.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "2024/25", DateTime.UtcNow.AddDays(-5), null, DateTime.UtcNow.AddDays(-5));
        typeof(RolloverWorkflowCandidate)
            .GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(candidate, id);

        var p1 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = id,
            FundingStream = null, 
            LatestFundingApprovalEndDate = DateTime.UtcNow.AddDays(-10),
            ThresholdDate = DateTime.UtcNow.AddDays(-5),
            OperationalEndDate = DateTime.UtcNow.AddDays(-20),
            OfferedInEngland = false, 
            Glh = 200, 
            Tqt = 10,
            IsOnDefundingList = true 
        };

        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([p1]);

        _repoMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([candidate]);

        IEnumerable<RolloverWorkflowCandidate>? updatedArg = null;
        _repoMock.Setup(r => r.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()))
                 .Callback<IEnumerable<RolloverWorkflowCandidate>, CancellationToken>((c, ct) => updatedArg = c)
                 .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _repoMock.Verify(r => r.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(updatedArg);
        var saved = updatedArg!.Single();
        Assert.False(saved.PassP1);
        Assert.NotNull(saved.P1FailureReason);

        Assert.Contains("Funding Stream out of scope for RollOver", saved.P1FailureReason);
        Assert.Contains("Funding Approval End Date is before the Threshold", saved.P1FailureReason);
        Assert.Contains("Operating End Date is before the Threshold", saved.P1FailureReason);
        Assert.Contains("Not Funded in England", saved.P1FailureReason);
        Assert.Contains("GLH > TQT", saved.P1FailureReason);
        Assert.Contains("Qualification is on Defunding (Defunded) List", saved.P1FailureReason);
    }

    [Fact]
    public async Task Handle_CandidateNotFound_DoesNotCallUpdate()
    {
        // Arrange
        var p1 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = Guid.NewGuid()
        };

        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([p1]);

        _repoMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(Enumerable.Empty<RolloverWorkflowCandidate>());

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _repoMock.Verify(r => r.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailureWithException()
    {
        // Arrange
        var ex = new InvalidOperationException("error");
        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ThrowsAsync(ex);

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("error", result.ErrorMessage);
        Assert.NotNull(result.InnerException);
        Assert.IsType<InvalidOperationException>(result.InnerException);
    }
}
