using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Sections
{
    public class GetApplicationSectionStatusByApplicationIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationPageRepository> _repositoryMock;
        private readonly GetApplicationSectionStatusByApplicationIdQueryHandler _handler;
        public GetApplicationSectionStatusByApplicationIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationPageRepository>>();
            _handler = _fixture.Create<GetApplicationSectionStatusByApplicationIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationSectionStatus_Is_Returned()
        {
            // Arrange
            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            Guid applicationId = Guid.NewGuid();

            var query = new GetApplicationSectionStatusByApplicationIdQuery(sectionId, formVersionId, applicationId);
            var response = new List<Data.Entities.Application.ApplicationPage>()
            {
                new()
            };

            _repositoryMock.Setup(x => x.GetBySectionIdAsync(sectionId, applicationId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetBySectionIdAsync(sectionId, applicationId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Pages.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            Guid applicationId = Guid.NewGuid();

            Exception ex = new Exception();

            var query = new GetApplicationSectionStatusByApplicationIdQuery(sectionId, formVersionId, applicationId);
            var response = new List<Data.Entities.Application.ApplicationPage>()
            {
                new()
            };

            _repositoryMock.Setup(x => x.GetBySectionIdAsync(sectionId, applicationId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetBySectionIdAsync(sectionId, applicationId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}