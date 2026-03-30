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

    [Fact]
    public async Task Handle_WhenCandidatePassesAndPldnsIsBeforeOperationalAndLatest_SetsProposedFundingEndDateToPldnsDate()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var pldnsDate = new DateTime(2024, 1, 1);
        var currentFundingEndDate = new DateTime(2024, 4, 30);

        var candidate = CreateCandidate(candidateId, currentFundingEndDate);

        var p1Check = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = candidateId,
            FundingStream = nameof(RolloverWorkflowCandidatesP1Checks.Age1416),
            Age1416 = pldnsDate,
            ThresholdDate = new DateTime(2023, 12, 31),
            LatestFundingApprovalEndDate = new DateTime(2024, 7, 1),
            OperationalEndDate = new DateTime(2024, 6, 1),
            OfferedInEngland = true,
            Glh = 1,
            Tqt = 2,
            IsOnDefundingList = false,
            AcademicYear = "2024/25"
        };

        _repoMock
            .Setup(x => x.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([p1Check]);

        _repoMock
            .Setup(x => x.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([candidate]);

        List<RolloverWorkflowCandidate>? updatedCandidates = null;
        _repoMock
            .Setup(x => x.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<RolloverWorkflowCandidate>, CancellationToken>((items, _) => updatedCandidates = items.ToList())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _repoMock.Verify(
            x => x.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotNull(updatedCandidates);
        var updatedCandidate = Assert.Single(updatedCandidates!);
        Assert.True(updatedCandidate.PassP1);
        Assert.Null(updatedCandidate.P1FailureReason);
        Assert.Equal(pldnsDate, updatedCandidate.ProposedFundingEndDate);
    }

    [Fact]
    public async Task Handle_WhenCandidatePassesAndPldnsIsAfterAcademicYearAndOperationalEndDateIsAfterLatest_SetsProposedFundingEndDateToLatestFundingApprovalEndDate()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var latestFundingApprovalEndDate = new DateTime(2021, 7, 15);
        var pldnsDate = new DateTime(2021, 8, 1);

        var candidate = CreateCandidate(candidateId, new DateTime(2021, 9, 1));

        var p1Check = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = candidateId,
            FundingStream = nameof(RolloverWorkflowCandidatesP1Checks.Age1416),
            Age1416 = pldnsDate,
            ThresholdDate = new DateTime(2020, 12, 31),
            LatestFundingApprovalEndDate = latestFundingApprovalEndDate,
            OperationalEndDate = new DateTime(2021, 9, 1),
            OfferedInEngland = true,
            Glh = 1,
            Tqt = 2,
            IsOnDefundingList = false,
            AcademicYear = "2020/21"
        };

        _repoMock
            .Setup(x => x.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([p1Check]);

        _repoMock
            .Setup(x => x.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([candidate]);

        List<RolloverWorkflowCandidate>? updatedCandidates = null;
        _repoMock
            .Setup(x => x.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<RolloverWorkflowCandidate>, CancellationToken>((items, _) => updatedCandidates = items.ToList())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(updatedCandidates);

        var updatedCandidate = Assert.Single(updatedCandidates!);
        Assert.True(updatedCandidate.PassP1);
        Assert.Equal(latestFundingApprovalEndDate, updatedCandidate.ProposedFundingEndDate);
    }

    [Fact]
    public async Task Handle_WhenCandidateFailsAndAlreadyHasProposedFundingEndDate_SetsProposedFundingEndDateToCurrentFundingEndDate()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var currentFundingEndDate = new DateTime(2024, 4, 30);
        var initialProposedFundingEndDate = new DateTime(2025, 1, 1);

        var candidate = CreateCandidate(candidateId, currentFundingEndDate, initialProposedFundingEndDate);

        var p1Check = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = candidateId,
            FundingStream = null,
            ThresholdDate = new DateTime(2024, 1, 1),
            LatestFundingApprovalEndDate = new DateTime(2024, 6, 1),
            OperationalEndDate = new DateTime(2024, 5, 1),
            OfferedInEngland = false,
            Glh = 10,
            Tqt = 5,
            IsOnDefundingList = true,
            AcademicYear = "2024/25"
        };

        _repoMock
            .Setup(x => x.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([p1Check]);

        _repoMock
            .Setup(x => x.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([candidate]);

        List<RolloverWorkflowCandidate>? updatedCandidates = null;
        _repoMock
            .Setup(x => x.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<RolloverWorkflowCandidate>, CancellationToken>((items, _) => updatedCandidates = items.ToList())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(updatedCandidates);

        var updatedCandidate = Assert.Single(updatedCandidates!);
        Assert.False(updatedCandidate.PassP1);
        Assert.Contains("Funding Stream out of scope for RollOver", updatedCandidate.P1FailureReason);
        Assert.Equal(currentFundingEndDate, updatedCandidate.ProposedFundingEndDate);
    }

    [Fact]
    public async Task Handle_WhenCandidateFailsWithNoProposedFundingEndDate_LeavesProposedFundingEndDateUnchanged()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var candidate = CreateCandidate(candidateId, new DateTime(2024, 4, 30));

        var p1Check = new RolloverWorkflowCandidatesP1Checks
        {
            WorkflowCandidateId = candidateId,
            FundingStream = null,
            ThresholdDate = new DateTime(2024, 1, 1),
            LatestFundingApprovalEndDate = new DateTime(2024, 6, 1),
            OperationalEndDate = new DateTime(2024, 5, 1),
            OfferedInEngland = false,
            Glh = 10,
            Tqt = 5,
            IsOnDefundingList = true,
            AcademicYear = "2024/25"
        };

        _repoMock
            .Setup(x => x.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([p1Check]);

        _repoMock
            .Setup(x => x.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([candidate]);

        List<RolloverWorkflowCandidate>? updatedCandidates = null;
        _repoMock
            .Setup(x => x.UpdateRolloverWorkflowCandidatesAsync(It.IsAny<IEnumerable<RolloverWorkflowCandidate>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<RolloverWorkflowCandidate>, CancellationToken>((items, _) => updatedCandidates = items.ToList())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(updatedCandidates);

        var updatedCandidate = Assert.Single(updatedCandidates!);
        Assert.False(updatedCandidate.PassP1);
        Assert.Null(updatedCandidate.ProposedFundingEndDate);
        Assert.Contains("Funding Stream out of scope for RollOver", updatedCandidate.P1FailureReason);
        Assert.Contains("Not Funded in England", updatedCandidate.P1FailureReason);
        Assert.Contains("GLH > TQT", updatedCandidate.P1FailureReason);
        Assert.Contains("Qualification is on Defunding (Defunded) List", updatedCandidate.P1FailureReason);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailureResponse()
    {
        // Arrange
        var exception = new InvalidOperationException("error");
        _repoMock
            .Setup(x => x.GetRolloverWorkflowCandidatesP1ChecksAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("error", result.ErrorMessage);
        Assert.Same(exception, result.InnerException);
    }

    private static RolloverWorkflowCandidate CreateCandidate(Guid candidateId, DateTime currentFundingEndDate, DateTime? proposedFundingEndDate = null)
    {
        var candidate = RolloverWorkflowCandidate.Create(
            workflowRunId: Guid.NewGuid(),
            rolloverCandidateRecordId: Guid.NewGuid(),
            qualificationVersionId: Guid.NewGuid(),
            fundingOfferId: Guid.NewGuid(),
            academicYear: "2024/25",
            currentFundingEndDate: currentFundingEndDate,
            proposedFundingEndDate: proposedFundingEndDate,
            createdAt: currentFundingEndDate.AddDays(-1));

        SetCandidateId(candidate, candidateId);

        return candidate;
    }

    private static void SetCandidateId(RolloverWorkflowCandidate candidate, Guid candidateId)
    {
        typeof(RolloverWorkflowCandidate)
            .GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(candidate, candidateId);
    }
}
