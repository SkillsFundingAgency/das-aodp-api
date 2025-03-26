using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Application.Queries.Application.Application;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Application
{
    public class GetApplicationFormPreviewByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationRepository> _repositoryMock;
        private readonly GetApplicationFormPreviewByIdQueryHandler _handler;
        public GetApplicationFormPreviewByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationRepository>>();
            _handler = _fixture.Create<GetApplicationFormPreviewByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationFormPreview_Is_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            var query = new GetApplicationFormPreviewByIdQuery(formVersionId);

            _repositoryMock.Setup(x => x.GetFormVersionIdForApplicationAsync(formVersionId))
                           .ReturnsAsync(formVersionId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetFormVersionIdForApplicationAsync(formVersionId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(formVersionId, result.Value.ApplicationId);
        }


        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            var ex = new Exception();

            var query = new GetApplicationFormPreviewByIdQuery(formVersionId);

            _repositoryMock.Setup(x => x.GetFormVersionIdForApplicationAsync(formVersionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetFormVersionIdForApplicationAsync(formVersionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}