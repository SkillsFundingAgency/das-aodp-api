using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Application.Section
{
    public class GetApplicationSectionByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ISectionRepository> _repositoryMock;
        private readonly GetApplicationSectionByIdQueryHandler _handler;
        public GetApplicationSectionByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<ISectionRepository>>();
            _handler = _fixture.Create<GetApplicationSectionByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationSection_Is_Returned()
        {
            // Arrange
            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            var query = new GetApplicationSectionByIdQuery(sectionId, formVersionId);
            var response = new Data.Entities.FormBuilder.Section()
            {
                Id = sectionId,
                FormVersionId = formVersionId,
                Pages = new()
                {
                    new()
                }
            };

            _repositoryMock.Setup(x => x.GetSectionByIdAsync(sectionId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetSectionByIdAsync(sectionId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Pages.Count, result.Value.Pages.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            var query = new GetApplicationSectionByIdQuery(sectionId, formVersionId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetSectionByIdAsync(sectionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetSectionByIdAsync(sectionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}