using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Application.Queries.Jobs;
using SFA.DAS.AODP.Data.Repositories.Jobs;

namespace SFA.DAS.AODP.Application.Tests.Queries.Jobs
{
    public class GetJobByNameQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IJobsRepository> _repositoryMock;
        private readonly GetJobByNameQueryQueryHandler _handler;
        public GetJobByNameQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IJobsRepository>>();
            _handler = _fixture.Create<GetJobByNameQueryQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Job_Is_Returned()
        {
            // Arrange
            string name = "Test";

            var query = new GetJobByNameQuery(name);
            var response = new Job()
            {
                Name = name
            };

            _repositoryMock.Setup(x => x.GetJobByNameAsync(name))
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetJobByNameAsync(name), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Name, result.Value.Name);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            string name = "Test";

            var query = new GetJobByNameQuery(name);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetJobByNameAsync(name))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetJobByNameAsync(name), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}