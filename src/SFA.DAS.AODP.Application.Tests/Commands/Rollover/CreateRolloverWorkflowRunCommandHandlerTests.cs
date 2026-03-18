using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Rollover;

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

        //[Fact]
        //public async Task Handle_ReturnsSuccess_WhenWorkflowRunAndCandidatesAreCreated()
        //{
        //    // Arrange
        //    var command = _fixture.Create<CreateRolloverWorkflowRunCommand>();
        //    command.RolloverCandidateIds = _fixture.CreateMany<Guid>(3).ToList();

        //    var workflowRun = RolloverWorkflowRun.Create(
        //        command.AcademicYear,
        //        command.SelectionMethod,
        //        command.FundingEndDateEligibilityThreshold,
        //        command.OperationalEndDateEligibilityThreshold,
        //        command.MaximumApprovalFundingEndDate,
        //        command.CreatedByUserName,
        //        DateTime.UtcNow);

        //    _repositoryMock
        //        .Setup(r => r.CreateRolloverWorkflowRunAsync(It.IsAny<RolloverWorkflowRun>(), It.IsAny<CancellationToken>()))
        //        .ReturnsAsync(workflowRun);

        //    _repositoryMock
        //        .Setup(r => r.AddWorkflowCandidatesAsync(workflowRun.Id, workflowRun.AcademicYear, command.RolloverCandidateIds, It.IsAny<CancellationToken>()))
        //        .Returns(Task.CompletedTask);

        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.True(result.Success);
        //    Assert.NotNull(result.Value);
        //    Assert.Null(result.ErrorMessage);
        //    Assert.Equal(workflowRun.Id, result.Value.RolloverWorkflowRunId);

        //    _repositoryMock.Verify(r =>
        //        r.CreateRolloverWorkflowRunAsync(It.IsAny<RolloverWorkflowRun>(), It.IsAny<CancellationToken>()),
        //        Times.Once);

        //    _repositoryMock.Verify(r =>
        //        r.AddWorkflowCandidatesAsync(workflowRun.Id, workflowRun.AcademicYear, command.RolloverCandidateIds, It.IsAny<CancellationToken>()),
        //        Times.Once);
        //}

        //[Fact]
        //public async Task Handle_ReturnsFailure_WhenCandidateListIsNull()
        //{
        //    // Arrange
        //    var command = _fixture.Create<CreateRolloverWorkflowRunCommand>();
        //    command.RolloverCandidateIds = null!;

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.False(result.Success);
        //    Assert.NotNull(result.ErrorMessage);
        //    Assert.IsType<InvalidOperationException>(result.InnerException);
        //}

        //[Fact]
        //public async Task Handle_ReturnsFailure_WhenCandidateListEmpty()
        //{
        //    // Arrange
        //    var command = _fixture.Create<CreateRolloverWorkflowRunCommand>();
        //    command.RolloverCandidateIds = new List<Guid>();

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.False(result.Success);
        //    Assert.NotNull(result.ErrorMessage);
        //    Assert.IsType<InvalidOperationException>(result.InnerException);
        //}

        //[Fact]
        //public async Task Handle_ReturnsLockedRecordException_WhenRepositoryThrowsRecordLocked()
        //{
        //    // Arrange
        //    var command = _fixture.Create<CreateRolloverWorkflowRunCommand>();
        //    command.RolloverCandidateIds = _fixture.CreateMany<Guid>(2).ToList();

        //    _repositoryMock
        //        .Setup(r => r.CreateRolloverWorkflowRunAsync(It.IsAny<RolloverWorkflowRun>(), It.IsAny<CancellationToken>()))
        //        .ThrowsAsync(new RecordLockedException());

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.False(result.Success);
        //    Assert.IsType<LockedRecordException>(result.InnerException);
        //}

        //[Fact]
        //public async Task Handle_ReturnsDependantNotFoundException_WhenRepositoryThrowsNoForeignKey()
        //{
        //    // Arrange
        //    var command = _fixture.Create<CreateRolloverWorkflowRunCommand>();
        //    command.RolloverCandidateIds = _fixture.CreateMany<Guid>(2).ToList();

        //    var foreignKey = Guid.NewGuid();

        //    _repositoryMock
        //        .Setup(r => r.CreateRolloverWorkflowRunAsync(
        //            It.IsAny<RolloverWorkflowRun>(),
        //            It.IsAny<CancellationToken>()))
        //        .ThrowsAsync(new NoForeignKeyException(foreignKey));

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.False(result.Success);
        //    Assert.NotNull(result.InnerException);
        //    Assert.IsType<DependantNotFoundException>(result.InnerException);

        //    var ex = (DependantNotFoundException)result.InnerException!;
        //    Assert.Equal(foreignKey, ex.DependantId);
        //}


        //[Fact]
        //public async Task Handle_ReturnsFailure_WhenUnexpectedExceptionThrown()
        //{
        //    // Arrange
        //    var command = _fixture.Create<CreateRolloverWorkflowRunCommand>();
        //    command.RolloverCandidateIds = _fixture.CreateMany<Guid>(2).ToList();

        //    _repositoryMock
        //        .Setup(r => r.CreateRolloverWorkflowRunAsync(It.IsAny<RolloverWorkflowRun>(), It.IsAny<CancellationToken>()))
        //        .ThrowsAsync(new Exception("Unexpected!"));

        //    // Act
        //    var result = await _handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.False(result.Success);
        //    Assert.NotNull(result.ErrorMessage);
        //    Assert.Equal("Unexpected!", result.ErrorMessage);
        //    Assert.IsType<Exception>(result.InnerException);
        //}
    }
}
