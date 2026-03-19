using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Rollover;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Rollover;
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
        var value = Assert.IsType<BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>>(ok.Value);
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
    public async Task GetRolloverCandidates_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        // Arrange
        var response = _fixture.Create<BaseMediatrResponse<GetRolloverCandidatesQueryResponse>>();
        response.Success = true;
        response.Value = new GetRolloverCandidatesQueryResponse
        {
            RolloverCandidates = _fixture.CreateMany<SFA.DAS.AODP.Models.Rollover.RolloverCandidate>(3).ToList()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetRolloverCandidatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetRolloverCandidates();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GetRolloverCandidatesQueryResponse>(ok.Value);
        Assert.Equal(3, value.RolloverCandidates.Count());
    }

    [Fact]
    public async Task GetRolloverCandidates_ReturnsStatusCode500_WhenMediatorReturnsFailure()
    {
        // Arrange
        var response = _fixture.Create<BaseMediatrResponse<GetRolloverCandidatesQueryResponse>>();
        response.Success = false;

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetRolloverCandidatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetRolloverCandidates();

        // Assert
        var status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, status.StatusCode);
    }

    [Fact]
    public async Task GetRolloverCandidates_ReturnsOk_WithEmptyList_WhenNoDataReturned()
    {
        // Arrange
        var response = new BaseMediatrResponse<GetRolloverCandidatesQueryResponse>
        {
            Success = true,
            Value = new GetRolloverCandidatesQueryResponse
            {
                RolloverCandidates = new List<SFA.DAS.AODP.Models.Rollover.RolloverCandidate>()
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetRolloverCandidatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetRolloverCandidates();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GetRolloverCandidatesQueryResponse>(ok.Value);
        Assert.NotNull(value.RolloverCandidates);
        Assert.Empty(value.RolloverCandidates);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRun_ReturnsOk_WithId_WhenSuccess()
    {
        // Arrange
        var createdId = Guid.NewGuid();

        var command = new CreateRolloverWorkflowRunCommand
        {
            AcademicYear = "2024/25",
            RolloverCandidateIds = new List<Guid> { Guid.NewGuid() }
        };

        var mediatorResponse = new BaseMediatrResponse<CreateRolloverWorkflowRunCommandResponse>
        {
            Success = true,
            Value = new CreateRolloverWorkflowRunCommandResponse
            {
                RolloverWorkflowRunId = createdId
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateRolloverWorkflowRunCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediatorResponse);

        // Act
        var result = await _controller.CreateRolloverWorkflowRun(command);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);

        var payload = Assert.IsType<CreateRolloverWorkflowRunCommandResponse>(ok.Value);
        Assert.Equal(createdId, payload.RolloverWorkflowRunId);

        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateRolloverWorkflowRunCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRun_Returns500_WhenMediatorReturnsFailure()
    {
        // Arrange
        var command = new CreateRolloverWorkflowRunCommand
        {
            AcademicYear = "2024/25",
            RolloverCandidateIds = new List<Guid> { Guid.NewGuid() }
        };

        var mediatorResponse = new BaseMediatrResponse<CreateRolloverWorkflowRunCommandResponse>
        {
            Success = false,
            ErrorMessage = "Unexpected failure",
            InnerException = new Exception("Unexpected failure")
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateRolloverWorkflowRunCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediatorResponse);

        // Act
        var result = await _controller.CreateRolloverWorkflowRun(command);

        // Assert
        if (result is ObjectResult objectResult)
        {
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }
        else if (result is StatusCodeResult statusCodeResult)
        {
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
        else
        {
            Assert.True(false, $"Expected 500 result, got {result?.GetType().FullName ?? "null"}");
        }

        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateRolloverWorkflowRunCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}