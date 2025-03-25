using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application;
using SFA.DAS.AODP.Application.Queries.Jobs;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Tests.Application.Queries
{
    public class GetJobRunByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IJobRunsRepository> _repositoryMock;
        private readonly GetJobRunByIdQueryHandler _handler;

        public GetJobRunByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IJobRunsRepository>>();
            _handler = _fixture.Create<GetJobRunByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_JobRuns_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetJobRunByIdQuery>();
            var jobRun = _fixture.Build<Data.Entities.Jobs.JobRun>()
                        .With(w => w.Job, new Job())                        
                        .Create();
            var response = _fixture.Create<BaseMediatrResponse<GetJobRunByIdQueryResponse>>();
            response.Success = true;          

            _repositoryMock.Setup(x => x.GetJobRunsById(query.Id))
                           .ReturnsAsync(jobRun);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetJobRunsById(query.Id), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(jobRun.Id, result.Value.Id);
        }        
    }
}


