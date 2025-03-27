using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Application.Queries.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.Tests.Queries.Jobs
{
    public class GetJobsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IJobsRepository> _repositoryMock;
        private readonly GetJobsQueryQueryHandler _handler;
        public GetJobsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IJobsRepository>>();
            _handler = _fixture.Create<GetJobsQueryQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Jobs_Are_Returned()
        {
            // Arrange
            var query = new GetJobsQuery();
            var response = new List<Job>()
            {
                new Job()
                {
                    Name = " "
                }
            };

            _repositoryMock.Setup(x => x.GetJobsAsync())
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetJobsAsync(), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Jobs.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = new GetJobsQuery();

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetJobsAsync())
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetJobsAsync(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}