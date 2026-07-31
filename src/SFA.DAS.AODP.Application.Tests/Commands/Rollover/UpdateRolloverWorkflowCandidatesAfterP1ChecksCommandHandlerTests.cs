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
    public async Task Handle_NoChecks_ReturnsSuccess_AndDoesNotSave()
    {
        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        _repoMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        Assert.True(result.Success);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CandidatePassesAllChecks_UpdatesCandidate()
    {
        var id = Guid.NewGuid();

        var candidate = CreateCandidate(id, DateTime.UtcNow.AddDays(-1));

        var p1 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = id,
            FundingStream = "FS",
            LatestFundingApprovalEndDate = null,
            FundingEndDateThreshold = DateTime.UtcNow.AddDays(-10),
            OperationalEndDate = null,
            OperationalEndDateThreshold = DateTime.UtcNow.AddDays(-10),
            OfferedInEngland = true,
            IntentionToSeekFundingInEngland = true,
            IsOnDefundingList = false
        };

        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([p1]);

        _repoMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([candidate]);

        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        Assert.True(result.Success);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.True(candidate.PassP1);
        Assert.Null(candidate.P1FailureReason);
    }

    [Fact]
    public async Task Handle_CandidateFailsAllChecks_SetsFailureReason()
    {
        var id = Guid.NewGuid();
        var candidate = CreateCandidate(id, DateTime.UtcNow.AddDays(-5));

        var p1 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = id,
            FundingStream = null,
            LatestFundingApprovalEndDate = DateTime.UtcNow.AddDays(-10),
            FundingEndDateThreshold = DateTime.UtcNow.AddDays(-5),
            OperationalEndDate = DateTime.UtcNow.AddDays(-20),
            OperationalEndDateThreshold = DateTime.UtcNow.AddDays(-18),
            OfferedInEngland = false,
            IntentionToSeekFundingInEngland = false,
            IsOnDefundingList = true
        };

        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([p1]);

        _repoMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([candidate]);

        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        Assert.True(result.Success);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.False(candidate.PassP1);
        Assert.NotNull(candidate.P1FailureReason);

        Assert.Contains("Funding Stream out of scope for RollOver", candidate.P1FailureReason);
        Assert.Contains("Funding Approval End Date is before the Threshold", candidate.P1FailureReason);
        Assert.Contains("Operating End Date is before the Threshold", candidate.P1FailureReason);
        Assert.Contains("Not Offered in England", candidate.P1FailureReason);
        Assert.Contains("Not Funded in England", candidate.P1FailureReason);
        Assert.Contains("Qualification is on Defunding (Defunded) List", candidate.P1FailureReason);
    }

    [Fact]
    public async Task Handle_CandidateNotFound_DoesNotSave()
    {
        var p1 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = Guid.NewGuid()
        };

        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([p1]);

        _repoMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        Assert.True(result.Success);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailure()
    {
        var ex = new InvalidOperationException("error");

        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ThrowsAsync(ex);

        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("error", result.ErrorMessage);
        Assert.Same(ex, result.InnerException);
    }

    [Fact]
    public async Task Handle_WhenCandidateFails_RevertsProposedFundingEndDateToCurrent()
    {
        var id = Guid.NewGuid();
        var current = new DateTime(2024, 4, 30);
        var initialProposed = new DateTime(2025, 1, 1);

        var candidate = CreateCandidate(id, current, initialProposed);

        var p1 = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = id,
            FundingStream = null,
            FundingEndDateThreshold = DateTime.UtcNow,
            OperationalEndDateThreshold = DateTime.UtcNow,
            OfferedInEngland = false,
            IsOnDefundingList = true
        };

        _repoMock.Setup(r => r.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([p1]);

        _repoMock.Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([candidate]);

        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(current, candidate.ProposedFundingEndDate);
    }

    private static RolloverWorkflowCandidate CreateCandidate(Guid id, DateTime currentFundingEnd, DateTime? proposedFundingEndDate = null)
    {
        var candidate = RolloverWorkflowCandidate.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2024/25",
            1,
            currentFundingEnd,
            proposedFundingEndDate,
            currentFundingEnd.AddDays(-1));

        typeof(RolloverWorkflowCandidate)
            .GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(candidate, id);

        return candidate;
    }
}
