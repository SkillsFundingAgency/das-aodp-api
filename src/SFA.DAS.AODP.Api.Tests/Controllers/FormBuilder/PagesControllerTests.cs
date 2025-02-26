using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Pages;

namespace SFA.DAS.AODP.Api.Tests.Controllers.FormBuilder.PagesControllerTests
{
    public class PagesControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<Api.Controllers.FormBuilder.PagesController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Api.Controllers.FormBuilder.PagesController _controller;

        public PagesControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.Controllers.FormBuilder.PagesController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new Api.Controllers.FormBuilder.PagesController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetAllPagesQuery>();
            var response = _fixture.Create<GetAllPagesQueryResponse>();
            BaseMediatrResponse<GetAllPagesQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllPagesQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetAllAsync(request.SectionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Never());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetAllPagesQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetPageByIdQuery>();
            var response = _fixture.Create<GetPageByIdQueryResponse>();
            BaseMediatrResponse<GetPageByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetPageByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetByIdAsync(request.PageId, request.SectionId, request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Never());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetPageByIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetPagePreviewByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetPagePreviewByIdQuery>();
            var response = _fixture.Create<GetPagePreviewByIdQueryResponse>();
            BaseMediatrResponse<GetPagePreviewByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetPagePreviewByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetPagePreviewByIdAsync(request.FormVersionId, request.PageId, request.SectionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Never());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetPagePreviewByIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<CreatePageCommand>();
            var response = _fixture.Create<CreatePageCommandResponse>();
            BaseMediatrResponse<CreatePageCommandResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreatePageCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.CreateAsync(request.FormVersionId, request.SectionId, request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CreatePageCommandResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<UpdatePageCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdatePageCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.UpdateAsync(request.FormVersionId, request.SectionId, request.Id, request);

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
            var request = _fixture.Create<MovePageUpCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MovePageUpCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.MoveUpAsync(request.FormVersionId, request.SectionId, request.PageId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Never());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task MoveDownAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<MovePageDownCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MovePageDownCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.MoveDownAsync(request.FormVersionId, request.SectionId, request.PageId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Never());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task RemoveAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<DeletePageCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeletePageCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.RemoveAsync(request.PageId);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Never());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }
    }
}