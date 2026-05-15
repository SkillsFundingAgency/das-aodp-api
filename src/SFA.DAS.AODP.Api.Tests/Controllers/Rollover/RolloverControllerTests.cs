using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Rollover;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.Rollover;

namespace SFA.DAS.AODP.Api.UnitTests.Controllers.Rollover;
public class RolloverControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<RolloverController>> _loggerMock;
    private readonly RolloverController _controller;

    public RolloverControllerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger<RolloverController>>>();
        _controller = new RolloverController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidates_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        // Arrange
        var response = _fixture.Create<BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>>();
        response.Success = true;
        response.Value = new GetRolloverWorkflowCandidatesCountQueryResponse
        {
            TotalRecords = 2
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolloverWorkflowCandidatesCountQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetRolloverWorkflowCandidatesCount(default);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType <BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>>(ok.Value);
        Assert.Equal(2, value.Value.TotalRecords);
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidates_ReturnsStatusCode_WhenMediatorReturnsFailure()
    {
        // Arrange
        var response = new BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>
        {
            Success = false,
            ErrorMessage = "Failed to get the count"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolloverWorkflowCandidatesCountQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetRolloverWorkflowCandidatesCount(default);

        // Assert
        var status = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>>(status.Value);
        Assert.False(value.Success);
        Assert.Equal("Failed to get the count", value.ErrorMessage);
    }

    [Fact]
    public async Task RolloverWorkflowCandidatesAfterP1Checks_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        // Arrange
        var command = new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand();
        var response = new BaseMediatrResponse<EmptyResponse>
        {
            Success = true,
            Value = new EmptyResponse()
        };

        _mediatorMock.Setup(m => m.Send(It.Is<UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand>(c => c == command), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RolloverWorkflowCandidatesAfterP1Checks(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<EmptyResponse>(okResult.Value);
    }

    [Fact]
    public async Task RolloverWorkflowCandidatesAfterP1Checks_ReturnsNotFound_WhenMediatorReturnsNotFoundException()
    {
        // Arrange
        var command = new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand();
        var response = new BaseMediatrResponse<EmptyResponse>
        {
            Success = false,
            InnerException = new NotFoundException(Guid.NewGuid())
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RolloverWorkflowCandidatesAfterP1Checks(command);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RolloverWorkflowCandidatesAfterP1Checks_ReturnsInternalServerError_WhenMediatorReturnsFailure()
    {
        // Arrange
        var command = new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand();
        var response = new BaseMediatrResponse<EmptyResponse>
        {
            Success = false,
            ErrorMessage = "Some error",
            InnerException = new Exception("Some error")
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.RolloverWorkflowCandidatesAfterP1Checks(command);

        // Assert
        var statusResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}
