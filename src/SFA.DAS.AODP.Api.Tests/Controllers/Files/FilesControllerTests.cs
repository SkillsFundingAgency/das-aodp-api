using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.Files;
using SFA.DAS.AODP.Application.Queries.Files;

namespace SFA.DAS.AODP.Api.Tests.Controllers.Files
{
    public class FilesControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<FilesController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly FilesController _controller;

        public FilesControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _loggerMock = _fixture.Freeze<Mock<ILogger<FilesController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();

            _controller = new FilesController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateFile_ReturnsOkResult_WhenMediatorReturnsSuccess()
        {
            // Arrange
            var command = _fixture.Create<CreateFileMetadataCommand>();
            var response = _fixture.Create<BaseMediatrResponse<EmptyResponse>>();
            response.Success = true;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateFileMetadataCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateFile(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var body = Assert.IsType<EmptyResponse>(okResult.Value);
        }

        [Fact]
        public async Task CreateFile_ReturnsStatusCode_WhenMediatorReturnsFailure()
        {
            // Arrange
            var command = _fixture.Create<CreateFileMetadataCommand>();
            var response = _fixture.Create<BaseMediatrResponse<EmptyResponse>>();
            response.Success = false;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateFileMetadataCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateFile(command);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
        }

        [Fact]
        public async Task DeleteFile_ReturnsOkResult_WhenMediatorReturnsSuccess()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var response = _fixture.Create<BaseMediatrResponse<EmptyResponse>>();
            response.Success = true;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteFileCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteFile(fileId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteFile_ReturnsStatusCode_WhenMediatorReturnsFailure()
        {
            // Arrange
            var fileId = Guid.NewGuid();
            var response = _fixture.Create<BaseMediatrResponse<EmptyResponse>>();
            response.Success = false;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteFileCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteFile(fileId);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
        }

        [Fact]
        public async Task GetFiles_ReturnsOkResult_WhenMediatorReturnsSuccess()
        {
            // Arrange
            var query = _fixture.Create<GetFileMetadataQuery>();
            var responsePayload = _fixture.Create<GetFileMetadataQueryResponse>();

            var response = new BaseMediatrResponse<GetFileMetadataQueryResponse>
            {
                Success = true,
                Value = responsePayload
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetFileMetadataQuery>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetFiles(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetFileMetadataQueryResponse>(okResult.Value);
        }

        [Fact]
        public async Task GetFiles_ReturnsStatusCode_WhenMediatorReturnsFailure()
        {
            // Arrange
            var query = _fixture.Create<GetFileMetadataQuery>();
            var response = _fixture.Create<BaseMediatrResponse<GetFileMetadataQueryResponse>>();
            response.Success = false;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetFileMetadataQuery>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetFiles(query);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
        }

        [Fact]
        public async Task GetFiles_ReturnsOk_WhenMediatorReturnsNullValue()
        {
            // Arrange
            var query = _fixture.Create<GetFileMetadataQuery>();

            var response = new BaseMediatrResponse<GetFileMetadataQueryResponse>
            {
                Success = true,
                Value = null
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetFileMetadataQuery>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetFiles(query);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}