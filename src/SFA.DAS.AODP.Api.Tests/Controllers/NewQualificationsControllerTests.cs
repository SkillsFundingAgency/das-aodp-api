using AutoFixture;
using AutoFixture.AutoMoq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Qualification;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Models.Qualifications;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Api.Tests.Controllers
{
    public class NewQualificationsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<NewQualificationsController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewQualificationsController _controller;

        public NewQualificationsControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<NewQualificationsController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new NewQualificationsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllNewQualifications_ReturnsOkResult_WithListOfNewQualifications()
        {
            // Arrange
            var queryResponse = _fixture
                .Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            queryResponse.Success = true;
            queryResponse.Value.NewQualifications = _fixture
                .CreateMany<NewQualification>(2).ToList();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetNewQualificationsQuery>(), default))
                .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetAllNewQualifications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BaseMediatrResponse<GetNewQualificationsQueryResponse>>(okResult.Value);
            Assert.Equal(2, model.Value.NewQualifications.Count);
            Assert.Equal(queryResponse.Value.NewQualifications[0].Title, model.Value.NewQualifications[0].Title);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Getting all new qualifications")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Successfully retrieved new qualifications")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllNewQualifications_ReturnsNotFound_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture
                .Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            queryResponse.Success = false;
            queryResponse.ErrorMessage = "Error";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetNewQualificationsQuery>(), default))
                .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetAllNewQualifications();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var notFoundValue = Assert.IsAssignableFrom<BaseMediatrResponse<GetNewQualificationsQueryResponse>>(notFoundResult.Value);
            Assert.Equal("Error", notFoundValue.ErrorMessage);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("No new qualifications found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsOkResult_WithQualificationDetails()
        {
            // Arrange
            var queryResponse = _fixture
                .Create<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>();
            queryResponse.Success = true;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetQualificationDetailsQuery>(), default))
                .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationDetails("Ref123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>(okResult.Value);
            Assert.Equal(queryResponse.Value.Id, model.Value.Id);
            Assert.Equal(queryResponse.Value.Status, model.Value.Status);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Getting details for qualification reference: Ref123")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Successfully retrieved details for qualification reference: Ref123")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsNotFound_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture
                .Create<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>();
            queryResponse.Success = false;
            queryResponse.ErrorMessage = "No details found for qualification reference: Ref123";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetQualificationDetailsQuery>(), default))
                .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationDetails("Ref123");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var notFoundValue = Assert.IsAssignableFrom<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>(notFoundResult.Value);
            Assert.Equal("No details found for qualification reference: Ref123", notFoundValue.ErrorMessage);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("No details found for qualification reference: Ref123")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsBadRequest_WhenQualificationReferenceIsEmpty()
        {
            // Act
            var result = await _controller.GetQualificationDetails(string.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestValue = Assert.IsAssignableFrom<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>(badRequestResult.Value);
            Assert.Equal("Qualification reference cannot be empty", badRequestValue.ErrorMessage);

            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Qualification reference is empty")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
