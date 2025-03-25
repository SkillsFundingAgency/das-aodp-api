using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Application;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Jobs;

namespace SFA.DAS.AODP.Api.Tests.Controllers.Jobs
{
    public class JobsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<JobsController>> _logger;
        private readonly JobsController _controller;
        private readonly Fixture _fixture;

        public JobsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _logger = new Mock<ILogger<JobsController>>();
            _fixture = new Fixture();
            _controller = new JobsController(_mediatorMock.Object, _logger.Object);
        }

        [Fact]
        public async Task GetJobs_ReturnsOkResult()
        {
            // Arrange
            var queryResponse = _fixture
                .Build<BaseMediatrResponse<GetJobsQueryResponse>>()
                .With(w => w.Success, true)
                .Create();

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetJobsQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(queryResponse));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetJobsQueryResponse>(okResult.Value);
            Assert.Equal(queryResponse.Value.Jobs.Count, model.Jobs.Count);
        }

        [Fact]
        public async Task GetJobs_Returns500_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture
                .Build<BaseMediatrResponse<GetJobsQueryResponse>>()
                .With(w => w.Success, false)
                .Create();

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetJobsQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(queryResponse));

            // Act
            var result = await _controller.GetAllAsync();

            // Assert
            var errorResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
        }

        [Fact]
        public async Task GetJobsByName_ReturnsOkResult()
        {
            // Arrange
            var queryResponse = _fixture
                .Build<BaseMediatrResponse<GetJobByNameQueryResponse>>()
                .With(w => w.Success, true)
                .Create();

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetJobByNameQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(queryResponse));
            var name = queryResponse.Value.Name;

            // Act
            var result = await _controller.GetByNameAsync(name);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetJobByNameQueryResponse>(okResult.Value);
            Assert.Equal(queryResponse.Value.Id, model.Id);
        }

        [Fact]
        public async Task GetJobsByName_Returns500_WhenQueryFails()
        {
            // Arrange
            var queryResponse = _fixture
                .Build<BaseMediatrResponse<GetJobByNameQueryResponse>>()
                .With(w => w.Success, false)
                .Create();

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetJobByNameQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(queryResponse));
            var name = queryResponse.Value.Name;

            // Act
            var result = await _controller.GetByNameAsync(name);

            // Assert
            var errorResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
        }
    }
}
