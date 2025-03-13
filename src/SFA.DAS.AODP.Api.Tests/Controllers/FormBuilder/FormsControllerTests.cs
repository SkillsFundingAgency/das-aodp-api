using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;

namespace SFA.DAS.AODP.Api.Tests.Controllers.FormBuilder
{
    public class FormsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<Api.Controllers.FormBuilder.FormsController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Api.Controllers.FormBuilder.FormsController _controller;

        public FormsControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.Controllers.FormBuilder.FormsController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new Api.Controllers.FormBuilder.FormsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetAllFormVersionsQuery>();
            var response = _fixture.Create<GetAllFormVersionsQueryResponse>();
            BaseMediatrResponse<GetAllFormVersionsQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllFormVersionsQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllFormVersionsQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.IsAny<GetAllFormVersionsQuery>(), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetAllFormVersionsQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetFormVersionByIdQuery>();
            var response = _fixture.Create<GetFormVersionByIdQueryResponse>();
            BaseMediatrResponse<GetFormVersionByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetFormVersionByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetByIdAsync(request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetFormVersionByIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetFormVersionByIdQuery>(q =>
                        q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetFormVersionByIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<CreateFormVersionCommand>();
            var response = _fixture.Create<CreateFormVersionCommandResponse>();
            BaseMediatrResponse<CreateFormVersionCommandResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateFormVersionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.CreateAsync(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CreateFormVersionCommandResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<UpdateFormVersionCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateFormVersionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.UpdateAsync(request.FormVersionId, request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task PublishAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<PublishFormVersionCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublishFormVersionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.PublishAsync(request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task UnpublishAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<UnpublishFormVersionCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UnpublishFormVersionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.UnpublishAsync(request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task MoveUpAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<MoveFormUpCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MoveFormUpCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.MoveUpAsync(request.FormId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task MoveDownAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<MoveFormDownCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MoveFormDownCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.MoveDownAsync(request.FormId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task CreateDraftAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<CreateDraftFormVersionCommand>();
            var response = _fixture.Create<CreateDraftFormVersionCommandResponse>();
            BaseMediatrResponse<CreateDraftFormVersionCommandResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateDraftFormVersionCommand>(), default))
                .Returns(Task.FromResult(wrapper));

            // Act
            var result = await _controller.CreateDraftAsync(request.FormId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CreateDraftFormVersionCommandResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task RemoveAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<DeleteFormVersionCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteFormVersionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.RemoveAsync(request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteFormVersionCommand>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<DeleteFormVersionCommand>(q =>
                        q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }
    }
}