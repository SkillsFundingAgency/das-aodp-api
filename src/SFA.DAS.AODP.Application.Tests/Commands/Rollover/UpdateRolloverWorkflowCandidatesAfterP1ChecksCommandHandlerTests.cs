using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Entities.Rollover.Enums;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;
using System.Reflection;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Rollover;

public class UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandlerTests : UnitTest
{
    private readonly Mock<IRolloverRepository> _repository = new();
    private readonly UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandler _handler;

    public UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandlerTests()
    {
        _handler = new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommandHandler(
            _repository.Object);
    }

    [Fact]
    public async Task Handle_WhenThereAreNoCandidates_ReturnsSuccessWithoutSaving()
    {
        // Arrange
        _repository
            .Setup(x => x.GetAllRolloverWorkflowCandidatesAsync(CancellationToken))
            .ReturnsAsync([]);
        _repository
            .Setup(x => x.GetRolloverCandidatesWithP1ChecksAsync(
                It.Is<IReadOnlyCollection<RolloverCandidateP1CheckRequest>>(
                    requests => requests.Count == 0),
                CancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(
            new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(),
            CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        _repository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCandidatePassesAllChecks_UpdatesAndSavesCandidate()
    {
        // Arrange
        var candidate = CreateCandidate(new DateTime(2025, 7, 31));
        var check = new RolloverWorkflowCandidatesP1Checks
        {
            RolloverCandidatesId = candidate.RolloverCandidatesId,
            FundingStream = "AdultSkills",
            FundingEndDateThreshold = new DateTime(2025, 1, 1),
            OperationalEndDateThreshold = new DateTime(2025, 1, 1),
            OfferedInEngland = true,
            IntentionToSeekFundingInEngland = true
        };
        SetupCandidateAndCheck(candidate, check);

        // Act
        var result = await _handler.Handle(
            new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(),
            CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        candidate.PassP1.ShouldBeTrue();
        candidate.P1FailureReason.ShouldBeNull();
        _repository.Verify(
            x => x.SaveChangesAsync(CancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCandidateFailsChecks_SetsFailureDetailsAndSavesCandidate()
    {
        // Arrange
        var candidate = CreateCandidate(new DateTime(2025, 7, 31));
        var check = new RolloverWorkflowCandidatesP1Checks
        {
            RolloverCandidatesId = candidate.RolloverCandidatesId,
            FundingStream = null,
            LatestFundingApprovalEndDate = new DateTime(2024, 1, 1),
            FundingEndDateThreshold = new DateTime(2025, 1, 1),
            OperationalEndDate = new DateTime(2024, 1, 1),
            OperationalEndDateThreshold = new DateTime(2025, 1, 1),
            OfferedInEngland = false,
            IntentionToSeekFundingInEngland = false,
            IsOnDefundingList = true
        };
        SetupCandidateAndCheck(candidate, check);

        // Act
        var result = await _handler.Handle(
            new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(),
            CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        candidate.PassP1.ShouldBeFalse();
        candidate.P1FailureReason.ShouldBe(
            "Funding Stream out of scope for RollOver; " +
            "Funding Approval End Date is before the Threshold; " +
            "Operating End Date is before the Threshold; " +
            "Not Offered in England; " +
            "Not Funded in England; " +
            "Qualification is on Defunding (Defunded) List");
        _repository.Verify(
            x => x.SaveChangesAsync(CancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoCheckIsReturnedForCandidate_DoesNotSave()
    {
        // Arrange
        var candidate = CreateCandidate(new DateTime(2025, 7, 31));
        _repository
            .Setup(x => x.GetAllRolloverWorkflowCandidatesAsync(CancellationToken))
            .ReturnsAsync([candidate]);
        _repository
            .Setup(x => x.GetRolloverCandidatesWithP1ChecksAsync(
                It.IsAny<IReadOnlyCollection<RolloverCandidateP1CheckRequest>>(),
                CancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(
            new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(),
            CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        _repository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCandidateFails_RevertsProposedFundingEndDateToCurrent()
    {
        // Arrange
        var currentFundingEndDate = new DateTime(2024, 4, 30);
        var candidate = CreateCandidate(
            currentFundingEndDate,
            new DateTime(2025, 1, 1));
        var check = new RolloverWorkflowCandidatesP1Checks
        {
            RolloverCandidatesId = candidate.RolloverCandidatesId,
            FundingStream = null,
            OfferedInEngland = false,
            IsOnDefundingList = true
        };
        SetupCandidateAndCheck(candidate, check);

        // Act
        var result = await _handler.Handle(
            new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(),
            CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        candidate.ProposedFundingEndDate.ShouldBe(currentFundingEndDate);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailure()
    {
        // Arrange
        var exception = new InvalidOperationException("Database unavailable.");
        _repository
            .Setup(x => x.GetAllRolloverWorkflowCandidatesAsync(CancellationToken))
            .ThrowsAsync(exception);

        // Act
        var result = await _handler.Handle(
            new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand(),
            CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe(exception.Message);
        result.InnerException.ShouldBeSameAs(exception);
    }

    private void SetupCandidateAndCheck(
        RolloverWorkflowCandidate candidate,
        RolloverWorkflowCandidatesP1Checks check)
    {
        _repository
            .Setup(x => x.GetAllRolloverWorkflowCandidatesAsync(CancellationToken))
            .ReturnsAsync([candidate]);
        _repository
            .Setup(x => x.GetRolloverCandidatesWithP1ChecksAsync(
                It.Is<IReadOnlyCollection<RolloverCandidateP1CheckRequest>>(requests =>
                    requests.Count == 1 &&
                    requests.Single().RolloverCandidateId ==
                    candidate.RolloverCandidatesId &&
                    requests.Single().FundingEndDateEligibilityThreshold ==
                    candidate.RolloverWorkflowRun.FundingEndDateEligibilityThreshold),
                CancellationToken))
            .ReturnsAsync([
                new RolloverCandidateP1CheckData(
                    new RolloverCandidateDto
                    {
                        Id = candidate.RolloverCandidatesId,
                        QualificationVersionId = candidate.QualificationVersionId,
                        FundingOfferId = candidate.FundingOfferId,
                        AcademicYear = candidate.AcademicYear,
                        RolloverRound = candidate.RolloverRound
                    },
                    check)
            ]);
        _repository
            .Setup(x => x.SaveChangesAsync(CancellationToken))
            .Returns(Task.CompletedTask);
    }

    private static RolloverWorkflowCandidate CreateCandidate(
        DateTime currentFundingEndDate,
        DateTime? proposedFundingEndDate = null)
    {
        var workflowRun = RolloverWorkflowRun.Create(
            "2024/25",
            SelectionMethod.QueryBuilder,
            new DateTime(2025, 1, 1),
            new DateTime(2025, 2, 1),
            new DateTime(2026, 7, 31),
            "test.user",
            new DateTime(2024, 1, 1));
        var candidate = RolloverWorkflowCandidate.Create(
            workflowRun.Id,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "2024/25",
            1,
            currentFundingEndDate,
            proposedFundingEndDate,
            new DateTime(2024, 1, 1));

        typeof(RolloverWorkflowCandidate)
            .GetProperty(
                nameof(RolloverWorkflowCandidate.RolloverWorkflowRun),
                BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(candidate, workflowRun);

        return candidate;
    }
}
