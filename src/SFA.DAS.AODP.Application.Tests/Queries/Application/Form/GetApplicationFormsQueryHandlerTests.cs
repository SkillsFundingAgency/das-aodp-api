using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Form
{
    public class GetApplicationFormsQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IFormVersionRepository> _repositoryMock;
        private readonly GetApplicationFormsQueryHandler _handler;
        public GetApplicationFormsQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IFormVersionRepository>>();
            _handler = _fixture.Create<GetApplicationFormsQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationForms_Are_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            var query = new GetApplicationFormsQuery();
            var response = new List<FormVersion>()
            {
                new FormVersion()
                {
                    Form = new()
                }
            };

            _repositoryMock.Setup(x => x.GetPublishedFormVersions())
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPublishedFormVersions(), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Forms.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            var query = new GetApplicationFormsQuery();

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetPublishedFormVersions())
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPublishedFormVersions(), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}