using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Rollover;

public class RemovePreviousWorkflowCandidatesCommandHandlerTests
{
    private readonly Mock<IRolloverRepository> _repoMock;
    private readonly RemovePreviousWorkflowCandidatesCommandHandler _handler;

    public RemovePreviousWorkflowCandidatesCommandHandlerTests()
    {
        _repoMock = new Mock<IRolloverRepository>();
        _handler = new RemovePreviousWorkflowCandidatesCommandHandler(_repoMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRepositoryRemoves_ReturnsSuccess()
    {
        _repoMock.Setup(r => r.DeleteAllWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new RemovePreviousWorkflowCandidatesCommand(), CancellationToken.None);

        result.Success.ShouldBeTrue();
        _repoMock.Verify(r => r.DeleteAllWorkflowCandidatesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsFailure()
    {
        var ex = new InvalidOperationException("error");
        _repoMock.Setup(r => r.DeleteAllWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                 .ThrowsAsync(ex);

        var result = await _handler.Handle(new RemovePreviousWorkflowCandidatesCommand(), CancellationToken.None);

        result.Success.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("error");
        result.InnerException.ShouldBe(ex);
    }
}