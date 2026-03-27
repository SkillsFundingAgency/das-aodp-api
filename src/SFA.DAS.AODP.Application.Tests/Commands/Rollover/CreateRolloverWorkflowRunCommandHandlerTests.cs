using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Rollover
{
    public class CreateRolloverWorkflowRunCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IRolloverRepository> _repositoryMock;
        private readonly CreateRolloverWorkflowRunCommandHandler _handler;

        public CreateRolloverWorkflowRunCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IRolloverRepository>>();
            _handler = _fixture.Create<CreateRolloverWorkflowRunCommandHandler>();
        }

        [Fact]
        public async Task Handle_ReturnsSuccess_WhenWorkflowRunAndDependenciesAreCreated()
        {
            // Arrange
            var command = _fixture.Build<CreateRolloverWorkflowRunCommand>()
                .With(x => x.RolloverCandidateIds, _fixture.CreateMany<Guid>(3).ToList())
                .With(x => x.FundingOfferIds, _fixture.CreateMany<Guid>(2).ToList())
                .Create();

            var candidates = command.RolloverCandidateIds
                .Select(id => new RolloverCandidate
                {
                    Id = id,
                    QualificationVersionId = Guid.NewGuid(),
                    FundingOfferId = Guid.NewGuid(),
                    AcademicYear = command.AcademicYear,
                    RolloverRound = 1,
                    PreviousFundingEndDate = DateTime.UtcNow.AddDays(-5),
                    NewFundingEndDate = DateTime.UtcNow.AddDays(10)
                })
                .ToList();

            _repositoryMock
                .Setup(r => r.GetRolloverCandidatesByIdsAsync(
                    command.RolloverCandidateIds,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            var workflowRun = RolloverWorkflowRun.Create(
                command.AcademicYear,
                command.SelectionMethod,
                command.FundingEndDateEligibilityThreshold,
                command.OperationalEndDateEligibilityThreshold,
                command.MaximumApprovalFundingEndDate,
                command.CreatedByUserName!,
                DateTime.UtcNow);

            var workflowRunId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowRunAsync(
                    It.IsAny<RolloverWorkflowRun>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(workflowRunId);

            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowCandidatesAsync(
                    It.IsAny<IEnumerable<Data.Entities.Rollover.RolloverWorkflowCandidate>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowRunFundingOffersAsync(
                    It.IsAny<IEnumerable<RolloverWorkflowRunFundingOffer>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(workflowRunId, result.Value!.RolloverWorkflowRunId);
            Assert.Null(result.ErrorMessage);

            _repositoryMock.Verify(r =>
                r.GetRolloverCandidatesByIdsAsync(command.RolloverCandidateIds, It.IsAny<CancellationToken>()),
                Times.Once);

            _repositoryMock.Verify(r =>
                r.CreateRolloverWorkflowRunAsync(
                    It.IsAny<RolloverWorkflowRun>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _repositoryMock.Verify(r =>
                r.CreateRolloverWorkflowCandidatesAsync(
                    It.IsAny<IEnumerable<Data.Entities.Rollover.RolloverWorkflowCandidate>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _repositoryMock.Verify(r =>
                r.CreateRolloverWorkflowRunFundingOffersAsync(
                    It.IsAny<IEnumerable<RolloverWorkflowRunFundingOffer>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailure_WhenCandidateListIsNull()
        {
            // Arrange
            var command = _fixture.Create<CreateRolloverWorkflowRunCommand>();
            command.RolloverCandidateIds = null!;

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
            Assert.IsType<InvalidOperationException>(result.InnerException);
        }

        [Fact]
        public async Task Handle_ReturnsFailure_WhenCandidateListEmpty()
        {
            // Arrange
            var command = _fixture.Create<CreateRolloverWorkflowRunCommand>();
            command.RolloverCandidateIds = new List<Guid>();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.ErrorMessage);
            Assert.IsType<InvalidOperationException>(result.InnerException);
        }

        [Fact]
        public async Task Handle_ReturnsLockedRecordException_WhenRepositoryThrowsRecordLocked()
        {
            // Arrange
            var command = _fixture.Build<CreateRolloverWorkflowRunCommand>()
                .With(c => c.RolloverCandidateIds, _fixture.CreateMany<Guid>(2).ToList())
                .With(c => c.FundingOfferIds, _fixture.CreateMany<Guid>(1).ToList())
                .Create();

            // Mock: return valid candidates so handler proceeds
            var candidates = command.RolloverCandidateIds
                .Select(id => new RolloverCandidate
                {
                    Id = id,
                    QualificationVersionId = Guid.NewGuid(),
                    FundingOfferId = Guid.NewGuid(),
                    AcademicYear = command.AcademicYear,
                    RolloverRound = 1,
                    PreviousFundingEndDate = DateTime.UtcNow.AddDays(-1),
                    NewFundingEndDate = DateTime.UtcNow.AddDays(10)
                })
                .ToList();

            _repositoryMock
                .Setup(r => r.GetRolloverCandidatesByIdsAsync(
                    command.RolloverCandidateIds,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            // Mock: workflow run creation throws
            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowRunAsync(
                    It.IsAny<RolloverWorkflowRun>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RecordLockedException());

            // Must mock downstream methods even if never reached
            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowCandidatesAsync(
                    It.IsAny<IEnumerable<Data.Entities.Rollover.RolloverWorkflowCandidate>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowRunFundingOffersAsync(
                    It.IsAny<IEnumerable<RolloverWorkflowRunFundingOffer>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.IsType<LockedRecordException>(result.InnerException);
        }


        [Fact]
        public async Task Handle_ReturnsFailure_WhenUnexpectedExceptionThrown()
        {
            // Arrange
            var command = _fixture.Build<CreateRolloverWorkflowRunCommand>()
                .With(c => c.RolloverCandidateIds, _fixture.CreateMany<Guid>(2).ToList())
                .With(c => c.FundingOfferIds, _fixture.CreateMany<Guid>(1).ToList())
                .Create();

            // Return valid rollover candidates so handler continues
            var candidates = command.RolloverCandidateIds
                .Select(id => new RolloverCandidate
                {
                    Id = id,
                    QualificationVersionId = Guid.NewGuid(),
                    FundingOfferId = Guid.NewGuid(),
                    AcademicYear = command.AcademicYear,
                    RolloverRound = 1,
                    PreviousFundingEndDate = DateTime.UtcNow.AddDays(-2),
                    NewFundingEndDate = DateTime.UtcNow.AddDays(5)
                })
                .ToList();

            _repositoryMock
                .Setup(r => r.GetRolloverCandidatesByIdsAsync(
                    command.RolloverCandidateIds,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            // THIS is the method we want to throw
            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowRunAsync(
                    It.IsAny<RolloverWorkflowRun>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected!"));

            // Must mock downstream calls (even though never reached)
            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowCandidatesAsync(
                    It.IsAny<IEnumerable<Data.Entities.Rollover.RolloverWorkflowCandidate>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowRunFundingOffersAsync(
                    It.IsAny<IEnumerable<RolloverWorkflowRunFundingOffer>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Unexpected!", result.ErrorMessage);
            Assert.IsType<Exception>(result.InnerException);
        }

        [Fact]
        public async Task Handle_ReturnsDependantNotFoundException_WhenRepositoryThrowsNoForeignKey()
        {
            // Arrange
            var command = _fixture.Build<CreateRolloverWorkflowRunCommand>()
                .With(c => c.RolloverCandidateIds, _fixture.CreateMany<Guid>(2).ToList())
                .With(c => c.FundingOfferIds, _fixture.CreateMany<Guid>(1).ToList())
                .Create();

            // Mock valid rollover candidates so the handler passes validation
            var candidates = command.RolloverCandidateIds
                .Select(id => new RolloverCandidate
                {
                    Id = id,
                    QualificationVersionId = Guid.NewGuid(),
                    FundingOfferId = Guid.NewGuid(),
                    AcademicYear = command.AcademicYear,
                    RolloverRound = 1,
                    PreviousFundingEndDate = DateTime.UtcNow.AddDays(-2),
                    NewFundingEndDate = DateTime.UtcNow.AddDays(10)
                })
                .ToList();

            _repositoryMock
                .Setup(r => r.GetRolloverCandidatesByIdsAsync(
                    command.RolloverCandidateIds,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            var foreignKey = Guid.NewGuid();

            // The specific repository call we want to throw
            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowRunAsync(
                    It.IsAny<RolloverWorkflowRun>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NoForeignKeyException(foreignKey));

            // Downstream methods must still be mocked
            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowCandidatesAsync(
                    It.IsAny<IEnumerable<Data.Entities.Rollover.RolloverWorkflowCandidate>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.CreateRolloverWorkflowRunFundingOffersAsync(
                    It.IsAny<IEnumerable<RolloverWorkflowRunFundingOffer>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.IsType<DependantNotFoundException>(result.InnerException);

            var ex = (DependantNotFoundException)result.InnerException!;
            Assert.Equal(foreignKey, ex.DependantId);
        }
    }
}
