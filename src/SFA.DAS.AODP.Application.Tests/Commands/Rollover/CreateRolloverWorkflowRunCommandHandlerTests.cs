using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Entities.Rollover.Enums;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Rollover;

public class CreateRolloverWorkflowRunCommandHandlerTests : UnitTest
{
    private readonly Mock<IRolloverRepository> _repository = new();
    private readonly CreateRolloverWorkflowRunCommandHandler _handler;

    public CreateRolloverWorkflowRunCommandHandlerTests()
    {
        _handler = new CreateRolloverWorkflowRunCommandHandler(_repository.Object);
    }

    [Fact]
    public async Task Handle_WhenCandidatesAreValid_AppliesP1ChecksAndCreatesWorkflowOnce()
    {
        // Arrange
        var command = CreateCommand();
        var candidates = CreateCandidates(command.RolloverCandidateIds, command.AcademicYear);
        _repository
            .Setup(x => x.GetRolloverCandidatesWithP1ChecksAsync(
                It.Is<IReadOnlyCollection<RolloverCandidateP1CheckRequest>>(requests =>
                    requests.Select(request => request.RolloverCandidateId)
                        .SequenceEqual(command.RolloverCandidateIds)),
                CancellationToken))
            .ReturnsAsync(CreateCandidatesWithChecks(candidates, command));
        _repository
            .Setup(x => x.CreateRolloverWorkflowAsync(
                It.IsAny<RolloverWorkflowRun>(),
                It.IsAny<IReadOnlyCollection<RolloverWorkflowCandidate>>(),
                It.IsAny<IReadOnlyCollection<RolloverWorkflowRunFundingOffer>>(),
                CancellationToken))
            .ReturnsAsync((
                RolloverWorkflowRun workflowRun,
                IReadOnlyCollection<RolloverWorkflowCandidate> _,
                IReadOnlyCollection<RolloverWorkflowRunFundingOffer> _,
                CancellationToken _) => workflowRun.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.RolloverWorkflowRunId.ShouldNotBe(Guid.Empty);

        _repository.Verify(x => x.CreateRolloverWorkflowAsync(
            It.Is<RolloverWorkflowRun>(run =>
                run.Id == result.Value.RolloverWorkflowRunId),
            It.Is<IReadOnlyCollection<RolloverWorkflowCandidate>>(workflowCandidates =>
                workflowCandidates.Count == candidates.Count &&
                workflowCandidates.All(candidate => candidate.PassP1)),
            It.Is<IReadOnlyCollection<RolloverWorkflowRunFundingOffer>>(fundingOffers =>
                fundingOffers.Count == command.FundingOfferIds.Count),
            CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCandidateListIsNull_ReturnsFailure()
    {
        // Arrange
        var command = CreateCommand();
        command.RolloverCandidateIds = null!;

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.InnerException.ShouldBeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WhenCandidateListIsEmpty_ReturnsFailure()
    {
        // Arrange
        var command = CreateCommand();
        command.RolloverCandidateIds = [];

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.InnerException.ShouldBeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WhenCandidateIsMissing_ReturnsFailureWithoutCreatingWorkflow()
    {
        // Arrange
        var command = CreateCommand();
        var candidates = CreateCandidates(
            command.RolloverCandidateIds.Take(1),
            command.AcademicYear);
        _repository
            .Setup(x => x.GetRolloverCandidatesWithP1ChecksAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidateP1CheckRequest>>(),
                CancellationToken))
            .ReturnsAsync(CreateCandidatesWithChecks(candidates, command));

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.InnerException.ShouldBeOfType<InvalidOperationException>();
        _repository.Verify(x => x.CreateRolloverWorkflowAsync(
            It.IsAny<RolloverWorkflowRun>(),
            It.IsAny<IReadOnlyCollection<RolloverWorkflowCandidate>>(),
            It.IsAny<IReadOnlyCollection<RolloverWorkflowRunFundingOffer>>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCandidateAndP1DataIsMissing_ReturnsFailureWithoutCreatingWorkflow()
    {
        // Arrange
        var command = CreateCommand();
        _repository
            .Setup(x => x.GetRolloverCandidatesWithP1ChecksAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidateP1CheckRequest>>(),
                CancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.InnerException.ShouldBeOfType<InvalidOperationException>();
        _repository.Verify(x => x.CreateRolloverWorkflowAsync(
            It.IsAny<RolloverWorkflowRun>(),
            It.IsAny<IReadOnlyCollection<RolloverWorkflowCandidate>>(),
            It.IsAny<IReadOnlyCollection<RolloverWorkflowRunFundingOffer>>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAtomicCreateIsLocked_ReturnsLockedRecordException()
    {
        // Arrange
        var command = CreateCommand();
        SetupCandidatesAndChecks(command);
        _repository
            .Setup(x => x.CreateRolloverWorkflowAsync(
                It.IsAny<RolloverWorkflowRun>(),
                It.IsAny<IReadOnlyCollection<RolloverWorkflowCandidate>>(),
                It.IsAny<IReadOnlyCollection<RolloverWorkflowRunFundingOffer>>(),
                CancellationToken))
            .ThrowsAsync(new RecordLockedException());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.InnerException.ShouldBeOfType<LockedRecordException>();
    }

    [Fact]
    public async Task Handle_WhenAtomicCreateHasMissingDependency_ReturnsDependantNotFoundException()
    {
        // Arrange
        var command = CreateCommand();
        var foreignKey = Guid.NewGuid();
        SetupCandidatesAndChecks(command);
        _repository
            .Setup(x => x.CreateRolloverWorkflowAsync(
                It.IsAny<RolloverWorkflowRun>(),
                It.IsAny<IReadOnlyCollection<RolloverWorkflowCandidate>>(),
                It.IsAny<IReadOnlyCollection<RolloverWorkflowRunFundingOffer>>(),
                CancellationToken))
            .ThrowsAsync(new NoForeignKeyException(foreignKey));

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        var exception = result.InnerException.ShouldBeOfType<DependantNotFoundException>();
        exception.DependantId.ShouldBe(foreignKey);
    }

    [Fact]
    public async Task Handle_WhenAtomicCreateThrowsUnexpectedException_ReturnsFailure()
    {
        // Arrange
        var command = CreateCommand();
        SetupCandidatesAndChecks(command);
        _repository
            .Setup(x => x.CreateRolloverWorkflowAsync(
                It.IsAny<RolloverWorkflowRun>(),
                It.IsAny<IReadOnlyCollection<RolloverWorkflowCandidate>>(),
                It.IsAny<IReadOnlyCollection<RolloverWorkflowRunFundingOffer>>(),
                CancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database unavailable."));

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Database unavailable.");
        result.InnerException.ShouldBeOfType<InvalidOperationException>();
    }

    private void SetupCandidatesAndChecks(CreateRolloverWorkflowRunCommand command)
    {
        var candidates = CreateCandidates(command.RolloverCandidateIds, command.AcademicYear);

        _repository
            .Setup(x => x.GetRolloverCandidatesWithP1ChecksAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidateP1CheckRequest>>(),
                CancellationToken))
            .ReturnsAsync(CreateCandidatesWithChecks(candidates, command));
    }

    private static CreateRolloverWorkflowRunCommand CreateCommand()
    {
        return new CreateRolloverWorkflowRunCommand
        {
            AcademicYear = "2025/26",
            SelectionMethod = SelectionMethod.QueryBuilder,
            RolloverCandidateIds = [Guid.NewGuid(), Guid.NewGuid()],
            FundingOfferIds = [Guid.NewGuid()],
            FundingEndDateEligibilityThreshold = new DateTime(2025, 1, 1),
            OperationalEndDateEligibilityThreshold = new DateTime(2025, 2, 1),
            MaximumApprovalFundingEndDate = new DateTime(2026, 7, 31),
            CreatedByUserName = "test.user"
        };
    }

    private static List<RolloverCandidateDto> CreateCandidates(
        IEnumerable<Guid> candidateIds,
        string academicYear)
    {
        return candidateIds
            .Select((id, index) => new RolloverCandidateDto
            {
                Id = id,
                QualificationVersionId = Guid.NewGuid(),
                FundingOfferId = Guid.NewGuid(),
                AcademicYear = academicYear,
                RolloverRound = index + 1,
                PreviousFundingEndDate = new DateTime(2025, 7, 31),
                NewFundingEndDate = new DateTime(2026, 7, 31)
            })
            .ToList();
    }

    private static List<RolloverCandidateP1CheckData> CreateCandidatesWithChecks(
        IEnumerable<RolloverCandidateDto> candidates,
        CreateRolloverWorkflowRunCommand command)
    {
        return candidates
            .Select(candidate => new RolloverCandidateP1CheckData(
                candidate,
                new RolloverWorkflowCandidatesP1Checks
                {
                    RolloverCandidatesId = candidate.Id,
                    QualificationVersionId = candidate.QualificationVersionId,
                    FundingOfferId = candidate.FundingOfferId,
                    AcademicYear = candidate.AcademicYear!,
                    FundingStream = "AdultSkills",
                    FundingEndDateThreshold = command.FundingEndDateEligibilityThreshold,
                    OperationalEndDateThreshold = command.OperationalEndDateEligibilityThreshold,
                    MaximumApprovalEndDate = command.MaximumApprovalFundingEndDate,
                    OfferedInEngland = true,
                    IntentionToSeekFundingInEngland = true
                }))
            .ToList();
    }
}
