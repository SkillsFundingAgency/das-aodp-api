using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Sections;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;

namespace SFA.DAS.AODP.Api.Tests.Controllers.FormBuilder.SectionsControllerTests
{
    public class SectionsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<Api.Controllers.FormBuilder.SectionsController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Api.Controllers.FormBuilder.SectionsController _controller;

        public SectionsControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.Controllers.FormBuilder.SectionsController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new Api.Controllers.FormBuilder.SectionsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetAllSectionsQuery>();
            var response = _fixture.Create<GetAllSectionsQueryResponse>();
            BaseMediatrResponse<GetAllSectionsQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllSectionsQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetAllAsync(request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllSectionsQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetAllSectionsQuery>(q =>
                        q.FormVersionId == request.FormVersionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetAllSectionsQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<GetSectionByIdQuery>();
            var response = _fixture.Create<GetSectionByIdQueryResponse>();
            BaseMediatrResponse<GetSectionByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetSectionByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetByIdAsync(request.SectionId, request.FormVersionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetSectionByIdQuery>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetSectionByIdQuery>(q =>
                        q.FormVersionId == request.FormVersionId
                        && q.SectionId == request.SectionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetSectionByIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<CreateSectionCommand>();
            var response = _fixture.Create<CreateSectionCommandResponse>();
            BaseMediatrResponse<CreateSectionCommandResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateSectionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.CreateAsync(request.FormVersionId, request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<CreateSectionCommandResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<UpdateSectionCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateSectionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.UpdateAsync(request.FormVersionId, request.Id, request);

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
            var request = _fixture.Create<MoveSectionUpCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MoveSectionUpCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.MoveUpAsync(request.FormVersionId, request.SectionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<MoveSectionUpCommand>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<MoveSectionUpCommand>(q =>
                        q.FormVersionId == request.FormVersionId
                        && q.SectionId == request.SectionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task MoveDownAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<MoveSectionDownCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<MoveSectionDownCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.MoveDownAsync(request.FormVersionId, request.SectionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<MoveSectionDownCommand>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<MoveSectionDownCommand>(q =>
                        q.FormVersionId == request.FormVersionId
                        && q.SectionId == request.SectionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task RemoveAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<DeleteSectionCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteSectionCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.RemoveAsync(request.SectionId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteSectionCommand>(), default), Times.Once());
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<DeleteSectionCommand>(q =>
                        q.SectionId == request.SectionId
            ), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }
    }
}