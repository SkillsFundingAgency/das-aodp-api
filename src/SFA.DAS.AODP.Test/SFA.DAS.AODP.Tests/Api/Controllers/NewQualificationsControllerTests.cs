using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.AODP.Api.Controllers;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Tests.Api.Controllers
{
    public class NewQualificationsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly NewQualificationsController _controller;

        public NewQualificationsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new NewQualificationsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAllNewQualifications_ReturnsOkResult_WithListOfNewQualifications()
        {
            // Arrange
            var queryResponse = new GetNewQualificationsQueryResponse
            {
                Success = true,
                NewQualifications = new List<NewQualification>
                {
                new NewQualification { Id = 1, Title = "Qualification 1", Reference = "Ref 1", AwardingOrganisation = "AO 1", Status = "Status 1" },
                new NewQualification { Id = 2, Title = "Qualification 2", Reference = "Ref 2", AwardingOrganisation = "AO 2", Status = "Status 2" }
                }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetNewQualificationsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetAllNewQualifications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetNewQualificationsQueryResponse>(okResult.Value);
            Assert.Equal(2, model.NewQualifications.Count);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsOkResult_WithQualificationDetails()
        {
            // Arrange
            var queryResponse = new GetQualificationDetailsQueryResponse
            {
                Success = true,
                Id = 1,
                Title = "Qualification 1"
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetQualificationDetailsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationDetails("test123");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetQualificationDetailsQueryResponse>(okResult.Value);
            Assert.Equal(1, model.Id);
            Assert.Equal("Qualification 1", model.Title);
        }

        [Fact]
        public async Task GetQualificationDetails_ReturnsNotFound_WhenQueryFails()
        {
            // Arrange
            var queryResponse = new GetQualificationDetailsQueryResponse { Success = false };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetQualificationDetailsQuery>(), default))
                         .ReturnsAsync(queryResponse);

            // Act
            var result = await _controller.GetQualificationDetails("test123");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
