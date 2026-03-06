using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Rollover;
using SFA.DAS.AODP.Application;
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
        var response = _fixture.Create<BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>>();
        response.Success = true;
        response.Value = new GetRolloverWorkflowCandidatesQueryResponse
        {
            Data = _fixture.CreateMany<SFA.DAS.AODP.Models.Rollover.RolloverWorkflowCandidate>(2).ToList(),
            Skip = 0,
            Take = 10,
            TotalRecords = 2
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolloverWorkflowCandidatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetRolloverWorkflowCandidates(0, 10);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GetRolloverWorkflowCandidatesQueryResponse>(ok.Value);
        Assert.Equal(2, value.Data.Count);
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidates_ReturnsStatusCode_WhenMediatorReturnsFailure()
    {
        // Arrange
        var response = _fixture.Create<BaseMediatrResponse<GetRolloverWorkflowCandidatesQueryResponse>>();
        response.Success = false;
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolloverWorkflowCandidatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetRolloverWorkflowCandidates(0, 10);

        // Assert
        var status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, status.StatusCode);
    }
}