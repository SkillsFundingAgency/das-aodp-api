using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Jobs;
using SFA.DAS.AODP.Application.Queries.Jobs;
using SFA.DAS.AODP.Application;

namespace SFA.DAS.AODP.Api.Tests.Controllers.JobRuns
{
    public class JobRunsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<JobRunsController>> _logger;
        private readonly JobRunsController _controller;
        private readonly Fixture _fixture;

        public JobRunsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _logger = new Mock<ILogger<JobRunsController>>();
            _fixture = new Fixture();
            _controller = new JobRunsController(_mediatorMock.Object, _logger.Object);
        }

        [Fact]
        public async Task GetJobRuns_ReturnsOkResult()
        {
            // Arrange
            var queryResponse = _fixture
                .Build<BaseMediatrResponse<GetJobRunsQueryResponse>>()
                .With(w => w.Success, true)
                .Create();

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetJobRunsQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(queryResponse));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetJobRunsQueryResponse>(okResult.Value);
            Assert.Equal(queryResponse.Value.JobRuns.Count, model.JobRuns.Count);
        }

        [Fact]
        public async Task GetJobRuns_Returns500_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture
                .Build<BaseMediatrResponse<GetJobRunsQueryResponse>>()
                .With(w => w.Success, false)
                .Create();

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetJobRunsQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(queryResponse));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var errorResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
        }
    }
}
