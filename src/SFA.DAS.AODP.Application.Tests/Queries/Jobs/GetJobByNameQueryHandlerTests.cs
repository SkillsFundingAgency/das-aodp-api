using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Jobs;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetJobByNameQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IJobsRepository> _repositoryMock;
        private readonly GetJobByNameQueryHandler _handler;

        public GetJobByNameQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IJobsRepository>>();
            _handler = _fixture.Create<GetJobByNameQueryHandler>();
        }

        [Fact]
        public async Task Then_JobData_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetJobByNameQuery>();
            var job = _fixture.Build<Data.Entities.Jobs.Job>()
                        .With(w => w.JobConfigurations, new List<JobConfiguration>())
                        .With(w => w.JobRuns, new List<JobRun>())
                        .Create();
            var response = _fixture.Create<BaseMediatrResponse<GetJobByNameQueryResponse>>();
            response.Success = true;          

            _repositoryMock.Setup(x => x.GetJobByNameAsync(query.Name))
                           .ReturnsAsync(job);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetJobByNameAsync(query.Name), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(job.Id, result.Value.Id);
        }        
    }
}


