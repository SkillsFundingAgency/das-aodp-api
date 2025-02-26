//using AutoFixture;
//using AutoFixture.AutoMoq;
//using MediatR;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Microsoft.AspNetCore.Mvc;
//using SFA.DAS.AODP.Application;
//using SFA.DAS.AODP.Application.Exceptions;
//using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
//using System.Net.NetworkInformation;

//namespace SFA.DAS.AODP.Api.Tests.Controllers.FormBuilder.PingControllerTests
//{
//    public class PingControllerTests
//    {
//        private readonly IFixture _fixture;
//        private readonly Mock<ILogger<Api.Controllers.FormBuilder.PingController>> _loggerMock;
//        private readonly Mock<IMediator> _mediatorMock;
//        private readonly Api.Controllers.FormBuilder.PingController _controller;

//        public PingControllerTests()
//        {
//            _fixture = new Fixture().Customize(new AutoMoqCustomization());
//            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.Controllers.FormBuilder.PingController>>>();
//            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
//            _controller = new Api.Controllers.FormBuilder.PingController(_mediatorMock.Object, _loggerMock.Object);
//        }

//        [Fact]
//        public async Task Ping()
//        {
//        //    // Arrange
//        //    var request = _fixture.Create<Ping>();
//        //    var response = _fixture.Create<EmptyResponse>();
//        //    BaseMediatrResponse<EmptyResponse> wrapper = new()
//        //    {
//        //        Value = response,
//        //        Success = true
//        //    };

//        //    _mediatorMock
//        //        .Setup(m => m.Send(It.IsAny<EmptyResponse>(), default))
//        //        .ReturnsAsync(wrapper);

//        //    // Act
//        //    var result = await _controller.Ping();

//        //    // Assert
//        //    _mediatorMock.Verify(m => m.Send(request, default), Times.Never());
//        //    var okResult = Assert.IsType<OkObjectResult>(result);
//        //    var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
//        //    Assert.Equal(response, model);
//        }
//    }
//}