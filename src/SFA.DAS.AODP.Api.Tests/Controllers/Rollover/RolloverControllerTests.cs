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
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.Rollover;
using SFA.DAS.AODP.Models.Rollover;

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
        var response = _fixture.Create<BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>>();
        response.Success = true;
        response.Value = new GetRolloverWorkflowCandidatesCountQueryResponse { TotalRecords = 2 };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolloverWorkflowCandidatesCountQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetRolloverWorkflowCandidatesCount(default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>>(ok.Value);
        Assert.Equal(2, value.Value.TotalRecords);
    }

    [Fact]
    public async Task GetRolloverWorkflowCandidates_ReturnsStatusCode_WhenMediatorReturnsFailure()
    {
        var response = new BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>
        {
            Success = false,
            ErrorMessage = "Failed to get the count"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolloverWorkflowCandidatesCountQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetRolloverWorkflowCandidatesCount(default);

        var status = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<BaseMediatrResponse<GetRolloverWorkflowCandidatesCountQueryResponse>>(status.Value);
        Assert.False(value.Success);
        Assert.Equal("Failed to get the count", value.ErrorMessage);
    }

    [Fact]
    public async Task RolloverWorkflowCandidatesAfterP1Checks_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        var command = new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand();
        var response = new BaseMediatrResponse<EmptyResponse> { Success = true, Value = new EmptyResponse() };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.RolloverWorkflowCandidatesAfterP1Checks(command);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<EmptyResponse>(okResult.Value);
    }

    [Fact]
    public async Task RolloverWorkflowCandidatesAfterP1Checks_ReturnsNotFound_WhenMediatorReturnsNotFoundException()
    {
        var command = new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand();
        var response = new BaseMediatrResponse<EmptyResponse>
        {
            Success = false,
            InnerException = new NotFoundException(Guid.NewGuid())
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.RolloverWorkflowCandidatesAfterP1Checks(command);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RolloverWorkflowCandidatesAfterP1Checks_ReturnsInternalServerError_WhenMediatorReturnsFailure()
    {
        var command = new UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand();
        var response = new BaseMediatrResponse<EmptyResponse>
        {
            Success = false,
            ErrorMessage = "Some error",
            InnerException = new Exception("Some error")
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateRolloverWorkflowCandidatesAfterP1ChecksCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.RolloverWorkflowCandidatesAfterP1Checks(command);

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetRolloverCandidates_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        var response = _fixture.Create<BaseMediatrResponse<GetRolloverCandidatesQueryResponse>>();
        response.Success = true;
        response.Value = new GetRolloverCandidatesQueryResponse
        {
            RolloverCandidates = _fixture.CreateMany<RolloverCandidateDto>(3).ToList()
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolloverCandidatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetRolloverCandidates();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GetRolloverCandidatesQueryResponse>(ok.Value);
        Assert.Equal(3, value.RolloverCandidates.Count());
    }

    [Fact]
    public async Task GetRolloverCandidates_ReturnsStatusCode500_WhenMediatorReturnsFailure()
    {
        var response = _fixture.Create<BaseMediatrResponse<GetRolloverCandidatesQueryResponse>>();
        response.Success = false;

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolloverCandidatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetRolloverCandidates();

        var status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, status.StatusCode);
    }

    [Fact]
    public async Task GetRolloverCandidates_ReturnsOk_WithEmptyList_WhenNoDataReturned()
    {
        var response = new BaseMediatrResponse<GetRolloverCandidatesQueryResponse>
        {
            Success = true,
            Value = new GetRolloverCandidatesQueryResponse
            {
                RolloverCandidates = new List<RolloverCandidateDto>()
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetRolloverCandidatesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetRolloverCandidates();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GetRolloverCandidatesQueryResponse>(ok.Value);
        Assert.Empty(value.RolloverCandidates);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRun_ReturnsOk_WithId_WhenSuccess()
    {
        var createdId = Guid.NewGuid();
        var command = new CreateRolloverWorkflowRunCommand
        {
            AcademicYear = "2024/25",
            RolloverCandidateIds = new List<Guid> { Guid.NewGuid() }
        };

        var mediatorResponse = new BaseMediatrResponse<CreateRolloverWorkflowRunCommandResponse>
        {
            Success = true,
            Value = new CreateRolloverWorkflowRunCommandResponse { RolloverWorkflowRunId = createdId }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateRolloverWorkflowRunCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediatorResponse);

        var result = await _controller.CreateRolloverWorkflowRun(command);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<CreateRolloverWorkflowRunCommandResponse>(ok.Value);
        Assert.Equal(createdId, payload.RolloverWorkflowRunId);
    }

    [Fact]
    public async Task CreateRolloverWorkflowRun_Returns500_WhenMediatorReturnsFailure()
    {
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

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateRolloverWorkflowRunCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediatorResponse);

        var result = await _controller.CreateRolloverWorkflowRun(command);

        var status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, status.StatusCode);
    }

    [Fact]
    public async Task GetRolloverCandidatesForExport_ReturnsOk_WhenMediatorReturnsSuccess()
    {
        var workflowRunId = Guid.NewGuid();

        var response = new BaseMediatrResponse<GetRolloverCandidatesForExportQueryResponse>
        {
            Success = true,
            Value = new GetRolloverCandidatesForExportQueryResponse
            {
                FileContent = new byte[] { 1, 2, 3 },
                FileName = "export.csv",
                ContentType = "text/csv"
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetRolloverCandidatesForExportQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetRolloverCandidatesForExport(
            workflowRunId,
            TestContext.Current.CancellationToken);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<GetRolloverCandidatesForExportQueryResponse>(ok.Value);

        Assert.Equal("export.csv", value.FileName);
        Assert.Equal("text/csv", value.ContentType);
        Assert.Equal(new byte[] { 1, 2, 3 }, value.FileContent);
    }

    [Fact]
    public async Task GetRolloverCandidatesForExport_ReturnsInternalServerError_WhenMediatorReturnsFailure()
    {
        var workflowRunId = Guid.NewGuid();

        var response = new BaseMediatrResponse<GetRolloverCandidatesForExportQueryResponse>
        {
            Success = false,
            ErrorMessage = "Something went wrong"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetRolloverCandidatesForExportQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.GetRolloverCandidatesForExport(
            workflowRunId,
            TestContext.Current.CancellationToken);

        var statusResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task ValidateFundingExtensionCandidates_ReturnsOk_WhenValidationSucceeds()
    {
        var command = _fixture.Create<ValidateFundingExtensionCandidatesCommand>();

        var mediatorResponse = new BaseMediatrResponse<ValidateFundingExtensionCandidatesCommandResponse>
        {
            Success = true,
            Value = new ValidateFundingExtensionCandidatesCommandResponse
            {
                IsValid = true,
                ValidationSuccessSummary = new FundingExtensionSummary
                {
                    TotalCandidatesCount = 20,
                    CandidatesExtendedInUploadCount = 10,
                    TotalCandidatesToBeExtendedCount = 5,
                    TotalCandidatesToBeExcludedCount = 5,
                    TotalCandidatesToBeReviewedCount = 10
                }
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<ValidateFundingExtensionCandidatesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediatorResponse);

        var result = await _controller.ValidateFundingExtensionCandidatesCommand(command);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ValidateFundingExtensionCandidatesCommandResponse>(ok.Value);

        Assert.True(payload.IsValid);
        Assert.NotNull(payload.ValidationSuccessSummary);
        Assert.Equal(20, payload.ValidationSuccessSummary.TotalCandidatesCount);
    }


    [Fact]
    public async Task ValidateFundingExtensionCandidates_ReturnsOk_WhenValidationFails()
    {
        var command = _fixture.Create<ValidateFundingExtensionCandidatesCommand>();

        var mediatorResponse = new BaseMediatrResponse<ValidateFundingExtensionCandidatesCommandResponse>
        {
            Success = true,
            Value = new ValidateFundingExtensionCandidatesCommandResponse
            {
                IsValid = false,
                ValidationFailureSummary = new ValidationFailureSummary
                {
                    FailedCandidateCount = 3,
                    ValidatedCandidateFile = new byte[] { 1, 2, 3 }
                }
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<ValidateFundingExtensionCandidatesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediatorResponse);

        var result = await _controller.ValidateFundingExtensionCandidatesCommand(command);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ValidateFundingExtensionCandidatesCommandResponse>(ok.Value);

        Assert.False(payload.IsValid);
        Assert.NotNull(payload.ValidationFailureSummary);
        Assert.Equal(3, payload.ValidationFailureSummary.FailedCandidateCount);
    }


    [Fact]
    public async Task ValidateFundingExtensionCandidates_Returns500_WhenMediatorReturnsFailure()
    {
        var command = _fixture.Create<ValidateFundingExtensionCandidatesCommand>();

        var mediatorResponse = new BaseMediatrResponse<ValidateFundingExtensionCandidatesCommandResponse>
        {
            Success = false,
            ErrorMessage = "Validation failed"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<ValidateFundingExtensionCandidatesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mediatorResponse);

        var result = await _controller.ValidateFundingExtensionCandidatesCommand(command);

        var status = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, status.StatusCode);
    }

}
