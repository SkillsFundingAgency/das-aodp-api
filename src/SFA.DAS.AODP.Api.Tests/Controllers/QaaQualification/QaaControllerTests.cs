using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.QaaQualification;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.QaaQualification;

namespace SFA.DAS.AODP.Api.UnitTests.Controllers.QaaQualification;

public class QaaControllerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<QaaController>> _loggerMock;
    private readonly QaaController _controller;

    public QaaControllerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger<QaaController>>>();
        _controller = new QaaController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetDownloadSummary_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        var response = new BaseMediatrResponse<GetQaaDownloadSummaryQueryResponse>
        {
            Success = true,
            Value = new GetQaaDownloadSummaryQueryResponse
            {
                NewQualificationsCount = 1,
                ExtendedQualificationsCount = 2,
                DiscontinuedQualificationsCount = 3
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetQaaDownloadSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetDownloadSummary();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GetQaaDownloadSummaryQueryResponse>(ok.Value);

        Assert.Equal(1, value.NewQualificationsCount);
        Assert.Equal(2, value.ExtendedQualificationsCount);
        Assert.Equal(3, value.DiscontinuedQualificationsCount);
    }

    [Fact]
    public async Task GetDownloadSummary_ReturnsInternalServerError_WhenMediatorReturnsFailure()
    {
        var response = new BaseMediatrResponse<GetQaaDownloadSummaryQueryResponse>
        {
            Success = false,
            ErrorMessage = "Something went wrong"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetQaaDownloadSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetDownloadSummary();

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task Download_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        var response = new BaseMediatrResponse<GetQaaQualificationsExportQueryResponse>
        {
            Success = true,
            Value = new GetQaaQualificationsExportQueryResponse
            {
                FileContent = new byte[] { 1, 2, 3 },
                FileName = "export.csv",
                ContentType = "text/csv"
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetQaaQualificationsExportQuery>(q => q.CurrentUsername == "tester"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.Download("tester");

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GetQaaQualificationsExportQueryResponse>(ok.Value);

        Assert.Equal("export.csv", value.FileName);
        Assert.Equal("text/csv", value.ContentType);
        Assert.Equal(new byte[] { 1, 2, 3 }, value.FileContent);
    }

    [Fact]
    public async Task Download_ReturnsInternalServerError_WhenMediatorReturnsFailure()
    {
        var response = new BaseMediatrResponse<GetQaaQualificationsExportQueryResponse>
        {
            Success = false,
            ErrorMessage = "Something went wrong"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetQaaQualificationsExportQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.Download("tester");

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}
