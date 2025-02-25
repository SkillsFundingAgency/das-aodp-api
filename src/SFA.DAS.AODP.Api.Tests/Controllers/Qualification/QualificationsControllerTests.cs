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

namespace SFA.DAS.AODP.Api.Tests.Controllers.Qualification
{
    public class QualificationsControllerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ILogger<QualificationsController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly QualificationsController _controller;

        public QualificationsControllerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _loggerMock = _fixture.Freeze<Mock<ILogger<QualificationsController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new QualificationsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetQualifications_ReturnsOkResult_WithListOfNewQualifications()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            queryResponse.Success = true;
            queryResponse.Value.NewQualifications = _fixture.CreateMany<NewQualification>(2).ToList();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualifications("new");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BaseMediatrResponse<GetNewQualificationsQueryResponse>>(okResult.Value);
            Assert.Equal(2, model.Value.NewQualifications.Count);
            Assert.Equal(queryResponse.Value.NewQualifications[0].Title, model.Value.NewQualifications[0].Title);
            Assert.Equal(queryResponse.Value.NewQualifications[0].Reference, model.Value.NewQualifications[0].Reference);
            Assert.Equal(queryResponse.Value.NewQualifications[0].AwardingOrganisation, model.Value.NewQualifications[0].AwardingOrganisation);
            Assert.Equal(queryResponse.Value.NewQualifications[0].Status, model.Value.NewQualifications[0].Status);
        }

        [Fact]
        public async Task GetQualifications_ReturnsNotFound_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            queryResponse.Success = false;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualifications("new");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var notFoundValue = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("No new qualifications found", notFoundValue);
        }

        [Fact]
        public async Task GetQualifications_ReturnsNotFound_WhenMediatorReturnsNull()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetNewQualificationsQueryResponse>>();
            queryResponse.Success = true;
            queryResponse.Value = null;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualifications("new");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var notFoundValue = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("No new qualifications found", notFoundValue);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsOkResult_WithQualificationDetails()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>();
            queryResponse.Success = true;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetQualificationDetailsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationDetails("Ref123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>(okResult.Value);
            Assert.Equal(queryResponse.Value.Id, model.Value.Id);
            Assert.Equal(queryResponse.Value.Status, model.Value.Status);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsNotFound_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture.Create<BaseMediatrResponse<GetQualificationDetailsQueryResponse>>();
            queryResponse.Success = false;
            queryResponse.ErrorMessage = "No details found for qualification reference: Ref123";

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetQualificationDetailsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationDetails("Ref123");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var notFoundValue = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null);
            Assert.Equal("No details found for qualification reference: Ref123", notFoundValue);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsBadRequest_WhenQualificationReferenceIsEmpty()
        {
            // Act
            var result = await _controller.GetQualificationDetails(string.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestValue = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null);
            Assert.Equal("Qualification reference cannot be empty", badRequestValue);
        }
    }
}


