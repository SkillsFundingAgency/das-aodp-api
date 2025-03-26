using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Form
{
    public class GetApplicationFormByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IFormVersionRepository> _repositoryMock;
        private readonly GetApplicationFormByIdQueryHandler _handler;
        public GetApplicationFormByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IFormVersionRepository>>();
            _handler = _fixture.Create<GetApplicationFormByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationForm_Is_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            var query = new GetApplicationFormByIdQuery(formVersionId);
            var response = new FormVersion()
            {
                Id = formVersionId,
                Sections = new()
                {
                    new()
                }
            };

            _repositoryMock.Setup(x => x.GetFormVersionByIdAsync(formVersionId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetFormVersionByIdAsync(formVersionId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Sections.Count, result.Value.Sections.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Exception ex = new Exception();

            var query = new GetApplicationFormByIdQuery(formVersionId);
            var response = new FormVersion()
            {
                Id = formVersionId,
                Sections = new()
                {
                    new()
                }
            };

            _repositoryMock.Setup(x => x.GetFormVersionByIdAsync(formVersionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetFormVersionByIdAsync(formVersionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}