using Moq;
using SFA.DAS.AODP.Application.Commands.Qualification;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Application.Review;

public class UpdateQualificationStatusCommandHandlerTests
{
    private readonly UpdateQualificationStatusCommandHandler _handler;
    private readonly Mock<IQualificationsRepository> _mockQualificationsRepository;
    public UpdateQualificationStatusCommandHandlerTests()
    {
        _mockQualificationsRepository = new Mock<IQualificationsRepository>();
        _handler = new UpdateQualificationStatusCommandHandler(_mockQualificationsRepository.Object);
    }

    [Fact]
    public async Task Handle_NullQualification_ReturnFalse()
    {
        // Arrange
        var command = new UpdateQualificationStatusCommand
        {
            ProcessStatusId = ProcessStatusLookup.Approved.Id,
            QualificationReference = "QAN123",
            Notes = "Approved",
            UserDisplayName = "Test User",
            Version = 1
        };

        // Expectations
        _mockQualificationsRepository.Setup(o => o.GetQualificationVersionByQanAsync("QAN123", It.IsAny<CancellationToken>())).ReturnsAsync((QualificationVersions?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(typeof(NotFoundWithNameException), result.InnerException!.GetType());
        Assert.NotEmpty(result.ErrorMessage!);
    }

    [Fact]
    public async Task Handle_ProcessStatusDifferentAsRequested_AddDiscussionHistory_SetStatus()
    {
        // Arrange
        var command = new UpdateQualificationStatusCommand
        {
            ProcessStatusId = ProcessStatusLookup.Approved.Id,
            QualificationReference = "QAN123",
            Notes = "Approved",
            UserDisplayName = "Test User",
            Version = 1
        };

        var discussionHistory = new QualificationDiscussionHistory
        {
            UserDisplayName = "Test User",
            Notes = "Approved",
            Title = "Updated status to: Approved"
        };

        var qualificationVersion = new QualificationVersions
        {
            LifecycleStageId = new Guid("00000000-0000-0000-0000-000000000001"),
            ProcessStatusId = ProcessStatusLookup.OnHold.Id,
            ProcessStatus = new ProcessStatus
            {
                Id = ProcessStatusLookup.OnHold.Id,
                Name = ProcessStatusLookup.OnHold.Name
            },
            LifecycleStage = new LifecycleStage
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Name = "New"
            }
        };

        // Expectations
        _mockQualificationsRepository.Setup(o => o.GetQualificationVersionByQanAsync("QAN123", It.IsAny<CancellationToken>())).ReturnsAsync(qualificationVersion);
        _mockQualificationsRepository.Setup(o => o.AddQualificationDiscussionHistory(discussionHistory, "QAN123")).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(ProcessStatusLookup.Approved.Id, qualificationVersion.ProcessStatusId);
        Assert.Equal(new Guid("00000000-0000-0000-0000-000000000003"), qualificationVersion.LifecycleStageId);
        _mockQualificationsRepository.Verify(o =>
            o.AddQualificationDiscussionHistory(It.IsAny<QualificationDiscussionHistory>(), "QAN123"), times: Times.Once);
    }

    [Fact]
    public async Task Handle_NoForeignKeyExceptionThrown_ReturnFalse()
    {
        // Arrange
        var command = new UpdateQualificationStatusCommand
        {
            ProcessStatusId = ProcessStatusLookup.Approved.Id,
            QualificationReference = "QAN123",
            Notes = "Approved",
            UserDisplayName = "Test User",
            Version = 1
        };

        // Expectations
        _mockQualificationsRepository.Setup(o => o.GetQualificationVersionByQanAsync("QAN123", It.IsAny<CancellationToken>())).ThrowsAsync(new NoForeignKeyException(Guid.NewGuid()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.False(result.Success);
        Assert.Equal(typeof(DependantNotFoundException), result.InnerException!.GetType());
        Assert.NotEmpty(result.ErrorMessage!);
    }

    [Fact]
    public async Task Handle_GenericExceptionThrown_ReturnFalse()
    {
        // Arrange
        var command = new UpdateQualificationStatusCommand
        {
            ProcessStatusId = ProcessStatusLookup.Approved.Id,
            QualificationReference = "QAN123",
            Notes = "Approved",
            UserDisplayName = "Test User",
            Version = 1
        };

        // Expectations
        _mockQualificationsRepository.Setup(o => o.GetQualificationVersionByQanAsync("QAN123", It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.False(result.Success);
        Assert.Equal(typeof(Exception), result.InnerException!.GetType());
        Assert.NotEmpty(result.ErrorMessage!);
    }
}