using AutoFixture;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Api.Controllers.Jobs;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.Jobs;

namespace SFA.DAS.AODP.Api.Tests.Controllers.Jobs
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
        public async Task GetJobs_ReturnsOkResult()
        {
            // Arrange
            var queryResponse = _fixture
                .Build<BaseMediatrResponse<GetJobRunsQueryResponse>>()
                .With(w => w.Success, true)
                .Create();
            var jobName = "Test";

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetJobRunsQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(queryResponse));

            // Act
            var result = await _controller.GetAllAsync(jobName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetJobRunsQueryResponse>(okResult.Value);
            Assert.Equal(queryResponse.Value.JobRuns.Count, model.JobRuns.Count);
        }
       
        [Fact]
        public async Task GetJobRunById_ReturnsOkResult()
        {
            // Arrange
            var queryResponse = _fixture
                .Build<BaseMediatrResponse<GetJobRunByIdQueryResponse>>()
                .With(w => w.Success, true)
                .Create();

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetJobRunByIdQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(queryResponse));
            var id = queryResponse.Value.Id;

            // Act
            var result = await _controller.GetJobRunByIdAsync(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<GetJobRunByIdQueryResponse>(okResult.Value);
            Assert.Equal(queryResponse.Value.Id, model.Id);
        }

        [Fact]
        public async Task RequestJobRunCommand_ReturnsOkResult()
        {
            // Arrange
            var queryResponse = _fixture
                .Build<BaseMediatrResponse<EmptyResponse>>()
                .With(w => w.Success, true)
                .Create();

            var command = _fixture
                .Build<RequestJobRunCommand>()                
                .Create();

            _mediatorMock.Setup(x => x.Send(It.IsAny<RequestJobRunCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(queryResponse));           

            // Act
            var result = await _controller.CreateAsync(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<EmptyResponse>(okResult.Value);
            Assert.NotNull(model);
        }
    }
}
