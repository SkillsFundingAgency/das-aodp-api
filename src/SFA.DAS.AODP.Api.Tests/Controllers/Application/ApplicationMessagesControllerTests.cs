using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Application.Message;

namespace SFA.DAS.AODP.Api.Tests.Controllers.Application
{
    public class ApplicationMessagesControllerTests
    {
        private IFixture _fixture;
        private Mock<ILogger<Api.Controllers.Application.ApplicationMessagesController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private Api.Controllers.Application.ApplicationMessagesController _controller;

        public ApplicationMessagesControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.Controllers.Application.ApplicationMessagesController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new Api.Controllers.Application.ApplicationMessagesController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetMessageByIdAsync_ReturnsOkResult()
        {
            // Arrange
            var response = _fixture.Create<GetApplicationMessageByIdQueryResponse>();
            BaseMediatrResponse<GetApplicationMessageByIdQueryResponse> wrapper = new()
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationMessageByIdQuery>(), default))
                .ReturnsAsync(wrapper);

            var msgId = Guid.NewGuid();

            // Act
            var result = await _controller.GetMessageByIdAsync(msgId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetApplicationMessageByIdQuery>(), default), Times.Once());
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationMessageByIdQueryResponse>(okResult.Value);
            Assert.Equal(response, model);
        }
    }

}