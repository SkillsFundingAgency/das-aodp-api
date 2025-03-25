using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Jobs;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetJobsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IJobsRepository> _repositoryMock;
        private readonly GetJobsQueryHandler _handler;

        public GetJobsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IJobsRepository>>();
            _handler = _fixture.Create<GetJobsQueryHandler>();
        }

        [Fact]
        public async Task Then_JobData_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetJobsQuery>();
            var jobs = _fixture.Build<Data.Entities.Jobs.Job>()
                        .With(w => w.JobConfigurations, new List<JobConfiguration>())
                        .With(w => w.JobRuns, new List<JobRun>())
                        .CreateMany(3);
            var response = _fixture.Create<BaseMediatrResponse<GetJobsQueryResponse>>();
            response.Success = true;

            _repositoryMock.Setup(x => x.GetJobsAsync()).ReturnsAsync(jobs.ToList());                           

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetJobsAsync(), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(3, result.Value.Jobs.Count());
        }        
    }
}


