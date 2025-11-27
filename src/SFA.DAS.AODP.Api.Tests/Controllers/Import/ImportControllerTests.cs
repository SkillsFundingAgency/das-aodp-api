using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Import;
using SFA.DAS.AODP.Api.Requests;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Import;

namespace SFA.DAS.AODP.Api.UnitTests.Controllers.Import;

public class ImportControllerTests
{
    private readonly Mock<ILogger<ImportController>> mockLogger = new();
    private readonly Mock<IMediator> mockMediator = new();

    [Fact]
    public async Task ImportDefundingList_ReturnsBadRequest_WhenFileIsNull()
    {
        // Arrange
        var controller = new ImportController(mockMediator.Object, mockLogger.Object);

        var request = new ImportDefundingListRequest
        {
            File = null
        };

        // Act
        var result = await controller.ImportDefundingList(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);

        var value = badRequest.Value as dynamic;
        var messageProp = value.GetType().GetProperty("message");
        var message = messageProp?.GetValue(value)?.ToString();
        Assert.Equal("No file uploaded.", message);
        mockMediator.Verify(m => m.Send(
                        It.IsAny<IRequest<BaseMediatrResponse<ImportDefundingListCommandResponse>>>(),
                        It.IsAny<CancellationToken>()),
                        Times.Never);
    }

    [Fact]
    public async Task ImportDefundingList_ReturnsBadRequest_WhenFileLengthIsZero()
    {
        // Arrange
        var controller = new ImportController(mockMediator.Object, mockLogger.Object);

        var emptyStream = new MemoryStream();
        var formFile = new FormFile(emptyStream, 0, 0, "file", "empty.csv");

        var request = new ImportDefundingListRequest
        {
            File = formFile
        };

        // Act
        var result = await controller.ImportDefundingList(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);

        var value = badRequest.Value as dynamic;
        var messageProp = value.GetType().GetProperty("message");
        var message = messageProp?.GetValue(value)?.ToString();
        Assert.Equal("No file uploaded.", message);
        mockMediator.Verify(m => m.Send(
                        It.IsAny<IRequest<BaseMediatrResponse<ImportDefundingListCommandResponse>>>(),
                        It.IsAny<CancellationToken>()),
                        Times.Never);
    }

    [Fact]
    public async Task ImportDefundingList_CallsMediatorAndReturnsOk_WhenFileProvided()
    {
        // Arrange
        var controller = new ImportController(mockMediator.Object, mockLogger.Object);

        var content = "id,name\n1,Test";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        var formFile = new FormFile(stream, 0, stream.Length, "file", "defunding.csv");

        var request = new ImportDefundingListRequest
        {
            File = formFile
        };

        var expectedResponse = new BaseMediatrResponse<ImportDefundingListCommandResponse>
        {
            Success = true,
            Value = new ImportDefundingListCommandResponse { ImportedCount = 1 }
        };

        ImportDefundingListCommand? capturedCommand = null;

        mockMediator
            .Setup(m => m.Send(
                            It.IsAny<IRequest<BaseMediatrResponse<ImportDefundingListCommandResponse>>>(),
                            It.IsAny<CancellationToken>()))
            .Callback<IRequest<BaseMediatrResponse<ImportDefundingListCommandResponse>>, CancellationToken>((r, ct) =>
            {
                capturedCommand = r as ImportDefundingListCommand;
            })
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.ImportDefundingList(request);

        mockMediator.Verify(m => m.Send(
                        It.IsAny<IRequest<BaseMediatrResponse<ImportDefundingListCommandResponse>>>(),
                        It.IsAny<CancellationToken>()),
                        Times.Once);

        Assert.NotNull(capturedCommand);
        Assert.Same(formFile, capturedCommand!.File);
        Assert.Equal(formFile.FileName, capturedCommand.FileName);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var response = Assert.IsType<ImportDefundingListCommandResponse>(okResult.Value);
        Assert.NotNull(response);
        Assert.Equal(1, response.ImportedCount);
    }

    [Fact]
    public async Task ImportPldns_ReturnsBadRequest_WhenFileIsNull()
    {
        // Arrange
        var controller = new ImportController(mockMediator.Object, mockLogger.Object);

        var request = new ImportPldnsRequest
        {
            File = null
        };

        // Act
        var result = await controller.ImportPldns(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);

        var value = badRequest.Value as dynamic;
        var messageProp = value.GetType().GetProperty("message");
        var message = messageProp?.GetValue(value)?.ToString();
        Assert.Equal("No file uploaded.", message);
        mockMediator.Verify(m => m.Send(
                        It.IsAny<IRequest<BaseMediatrResponse<ImportPldnsCommandResponse>>>(),
                        It.IsAny<CancellationToken>()),
                        Times.Never);
    }

    [Fact]
    public async Task ImportPldns_ReturnsBadRequest_WhenFileLengthIsZero()
    {
        // Arrange
        var controller = new ImportController(mockMediator.Object, mockLogger.Object);

        var emptyStream = new MemoryStream();
        var formFile = new FormFile(emptyStream, 0, 0, "file", "empty.csv");

        var request = new ImportPldnsRequest
        {
            File = formFile
        };

        // Act
        var result = await controller.ImportPldns(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequest.Value);

        var value = badRequest.Value as dynamic;
        var messageProp = value.GetType().GetProperty("message");
        var message = messageProp?.GetValue(value)?.ToString();
        Assert.Equal("No file uploaded.", message);
        mockMediator.Verify(m => m.Send(
                        It.IsAny<IRequest<BaseMediatrResponse<ImportPldnsCommandResponse>>>(),
                        It.IsAny<CancellationToken>()),
                        Times.Never);
    }

    [Fact]
    public async Task ImportPldns_CallsMediatorAndReturnsOk_WhenFileProvided()
    {
        // Arrange
        var controller = new ImportController(mockMediator.Object, mockLogger.Object);

        var content = "id,name\n1,Test";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        var formFile = new FormFile(stream, 0, stream.Length, "file", "pldns.csv");

        var request = new ImportPldnsRequest
        {
            File = formFile
        };

        var expectedResponse = new BaseMediatrResponse<ImportPldnsCommandResponse>
        {
            Success = true,
            Value = new ImportPldnsCommandResponse { ImportedCount = 1 }
        };

        ImportPldnsCommand? capturedCommand = null;

        mockMediator
            .Setup(m => m.Send(
                            It.IsAny<IRequest<BaseMediatrResponse<ImportPldnsCommandResponse>>>(),
                            It.IsAny<CancellationToken>()))
            .Callback<IRequest<BaseMediatrResponse<ImportPldnsCommandResponse>>, CancellationToken>((r, ct) =>
            {
                capturedCommand = r as ImportPldnsCommand;
            })
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.ImportPldns(request);

        mockMediator.Verify(m => m.Send(
                        It.IsAny<IRequest<BaseMediatrResponse<ImportPldnsCommandResponse>>>(),
                        It.IsAny<CancellationToken>()),
                        Times.Once);

        Assert.NotNull(capturedCommand);
        Assert.Same(formFile, capturedCommand!.File);
        Assert.Equal(formFile.FileName, capturedCommand.FileName);

        var okResult = Assert.IsType<OkObjectResult>(result);

        var response = Assert.IsType<ImportPldnsCommandResponse>(okResult.Value);
        Assert.NotNull(response);
        Assert.Equal(1, response.ImportedCount);
    }
}
