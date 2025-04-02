using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Application;
using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Api.Tests.Controllers.Application
{
    public class SurveyControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<SurveyController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly SurveyController _controller;

        public SurveyControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<SurveyController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new SurveyController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SaveSurveyAsync_ReturnsOkResult()
        {
            // Arrange
            var request = _fixture.Create<SaveSurveyCommand>();
            var response = _fixture.Create<EmptyResponse>();
            BaseMediatrResponse<EmptyResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SaveSurveyCommand>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.SaveSurveyAsync(request);

            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.Equal(response, model);
        }

        [Fact]
        public async Task SaveSurveyAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = _fixture.Create<SaveSurveyCommand>();
            var exception = new Exception("Test exception");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SaveSurveyCommand>(), default))
                .ThrowsAsync(exception);

            // Act & Assert

            await Assert.ThrowsAsync<Exception>(async () => await _controller.SaveSurveyAsync(request));


            // Assert
            _mediatorMock.Verify(m => m.Send(request, default), Times.Once());
        }
    }
}


