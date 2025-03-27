using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Form
{
    public class GetApplicationFormStatusByApplicationIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationRepository> _repositoryMock;
        private readonly Mock<IApplicationPageRepository> _repositoryPageMock;
        private readonly GetApplicationFormStatusByApplicationIdQueryHandler _handler;
        public GetApplicationFormStatusByApplicationIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationRepository>>();
            _handler = _fixture.Create<GetApplicationFormStatusByApplicationIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationFormStatus_Is_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid applicationId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            var query = new GetApplicationFormStatusByApplicationIdQuery(formVersionId, applicationId);
            query.FormVersionId = formVersionId;
            query.ApplicationId = applicationId;
            var response = new List<View_SectionSummaryForApplication>()
            {
                new View_SectionSummaryForApplication()
                {
                    SectionId = sectionId, 
                    ApplicationId = applicationId
                }
            };

            _repositoryMock.Setup(x => x.GetSectionSummaryByApplicationIdAsync(applicationId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetSectionSummaryByApplicationIdAsync(applicationId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Sections.Count);
            Assert.Single(result.Value.Sections);
            Assert.Equal(response[0].SectionId, result.Value.Sections[0].SectionId);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid applicationId = Guid.NewGuid();

            var query = new GetApplicationFormStatusByApplicationIdQuery(formVersionId, applicationId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetSectionSummaryByApplicationIdAsync(applicationId))
                           .ThrowsAsync(ex);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetSectionSummaryByApplicationIdAsync(applicationId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}