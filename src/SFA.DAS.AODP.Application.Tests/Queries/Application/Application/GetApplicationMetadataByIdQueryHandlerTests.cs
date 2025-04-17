using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Application.Application
{
    public class GetApplicationMetadataByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationRepository> _repositoryMock;
        private readonly GetApplicationMetadataByIdQueryHandler _handler;
        public GetApplicationMetadataByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationRepository>>();
            _handler = _fixture.Create<GetApplicationMetadataByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationMetadata_Is_Returned()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();

            var query = new GetApplicationMetadataByIdQuery(applicationId);
            var response = new Data.Entities.Application.Application()
            {
                Id = applicationId
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(applicationId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetByIdAsync(applicationId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Id, result.Value.ApplicationId);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();

            var query = new GetApplicationMetadataByIdQuery(applicationId);
            var response = new Data.Entities.Application.Application()
            {
                Id = applicationId
            };

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetByIdAsync(applicationId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetByIdAsync(applicationId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}