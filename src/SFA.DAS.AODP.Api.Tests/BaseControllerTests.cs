using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Api.Tests.BaseController
{
    public class FormsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<Api.BaseController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Api.BaseController _controller;

        public FormsControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.BaseController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new Api.BaseController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Get_Response_Success()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationFormByIdQuery>();
            var response = _fixture.Create<GetApplicationFormByIdQueryResponse>();
            BaseMediatrResponse<GetApplicationFormByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormByIdQuery>(), default))
                .ReturnsAsync(wrapper);
            
            // Act
            var result = await _controller.SendRequestAsync(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationFormByIdQueryResponse>(okResult.Value);
            Assert.Equal(response.FormTitle, model.FormTitle);
            Assert.Equal(response.Sections, model.Sections);
        }

        [Fact]
        public async Task Get_Response_NotFoundException()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationFormByIdQuery>();
            var response = _fixture.Create<GetApplicationFormByIdQueryResponse>();
            BaseMediatrResponse<GetApplicationFormByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = false,
                InnerException = new NotFoundException(request.FormVersionId)
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.SendRequestAsync(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Get_Response_LockedRecordException()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationFormByIdQuery>();
            var response = _fixture.Create<GetApplicationFormByIdQueryResponse>();
            BaseMediatrResponse<GetApplicationFormByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = false,
                InnerException = new LockedRecordException()
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.SendRequestAsync(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Get_Response_DependentNotFoundException()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationFormByIdQuery>();
            var response = _fixture.Create<GetApplicationFormByIdQueryResponse>();
            BaseMediatrResponse<GetApplicationFormByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = false,
                InnerException = new DependantNotFoundException(request.FormVersionId)
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.SendRequestAsync(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Get_Response_ErrorThrownHandlingRequest()
        {
            // Arrange
            var request = _fixture.Create<GetApplicationFormByIdQuery>();
            var response = _fixture.Create<GetApplicationFormByIdQueryResponse>();
            BaseMediatrResponse<GetApplicationFormByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = false,
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationFormByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.SendRequestAsync(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<StatusCodeResult>(result);
        }
    }
}