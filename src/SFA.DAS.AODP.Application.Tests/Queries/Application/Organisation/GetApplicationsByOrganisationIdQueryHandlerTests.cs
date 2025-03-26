using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Organisation
{
    public class GetApplicationsByOrganisationIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationRepository> _repositoryMock;
        private readonly GetApplicationsByOrganisationIdQueryHandler _handler;
        public GetApplicationsByOrganisationIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationRepository>>();
            _handler = _fixture.Create<GetApplicationsByOrganisationIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Organisation_Is_Returned()
        {
            // Arrange
            Guid organisationId = Guid.NewGuid();

            var query = new GetApplicationsByOrganisationIdQuery(organisationId);
            var response = new List<Data.Entities.Application.Application>()
            {
                new()
            };

            _repositoryMock.Setup(x => x.GetByOrganisationId(organisationId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetByOrganisationId(organisationId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Applications.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid organisationId = Guid.NewGuid();

            var query = new GetApplicationsByOrganisationIdQuery(organisationId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetByOrganisationId(organisationId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetByOrganisationId(organisationId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}