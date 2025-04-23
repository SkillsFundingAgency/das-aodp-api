using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Jobs;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Jobs
{
    public class GetJobRunsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IJobRunsRepository> _repositoryMock;
        private readonly GetJobRunsQueryHandler _handler;

        public GetJobRunsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IJobRunsRepository>>();
            _handler = _fixture.Create<GetJobRunsQueryHandler>();
        }

        [Fact]
        public async Task Then_JobRuns_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetJobRunsQuery>();
            var jobRuns = _fixture.Build<JobRun>()
                        .With(w => w.Job, new Job())                        
                        .CreateMany(3);
            var response = _fixture.Create<BaseMediatrResponse<GetJobRunsQueryResponse>>();
            response.Success = true;          

            _repositoryMock.Setup(x => x.GetJobRunsAsync(query.JobName))
                           .ReturnsAsync(jobRuns.ToList());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetJobRunsAsync(query.JobName), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(3, result.Value.JobRuns.Count());
        }        
    }
}


