using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Application.Review;
using SFA.DAS.AODP.Shared.UnitTests.Helpers;


namespace SFA.DAS.AODP.Api.UnitTests.Controllers.Application
{
    public class ApplicationsReviewsControllerTests
    {

        private readonly IFixture _fixture;
        private readonly Mock<ILogger<Api.Controllers.Application.ApplicationsReviewsController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Api.Controllers.Application.ApplicationsReviewsController _controller;


        public ApplicationsReviewsControllerTests()
        {   
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());

            _loggerMock = _fixture.Freeze<Mock<ILogger<Api.Controllers.Application.ApplicationsReviewsController>>>();
            _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
            _controller = new Api.Controllers.Application.ApplicationsReviewsController(_mediatorMock.Object, _loggerMock.Object);
        }


        [Fact]
        public async Task GetApplicationExportData_ReturnsOkResult()
        {
            // Arrange
            var applicationReviewId = Guid.NewGuid();

            var response = _fixture.Create<GetApplicationExportDataQueryResponse>();

            var wrapper = new BaseMediatrResponse<GetApplicationExportDataQueryResponse>
            {
                Value = response,
                Success = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationExportDataQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetApplicationExportData(applicationReviewId);

            // Assert
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationExportDataQuery>(q =>
                        q.ApplicationReviewId == applicationReviewId),
                    default),
                Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetApplicationExportDataQueryResponse>(okResult.Value);

            Assert.Equal(response, model);
        }

        [Fact]
        public async Task GetApplicationExportData_WhenMediatorFails_ReturnsInternalServerError()
        {
            // Arrange
            var applicationReviewId = Guid.NewGuid();

            var wrapper = new BaseMediatrResponse<GetApplicationExportDataQueryResponse>
            {
                Success = false,
                ErrorMessage = "Something went wrong"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetApplicationExportDataQuery>(), default))
                .ReturnsAsync(wrapper);

            // Act
            var result = await _controller.GetApplicationExportData(applicationReviewId);

            // Assert
            _mediatorMock.Verify(m =>
                m.Send(
                    It.Is<GetApplicationExportDataQuery>(q =>
                        q.ApplicationReviewId == applicationReviewId),
                    default),
                Times.Once);

            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        }
    }
}
