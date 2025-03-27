using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;

namespace SFA.DAS.AODP.Application.Tests.Queries.FormBuilder.Pages
{
    public class GetAllPagesQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IPageRepository> _repositoryMock;
        private readonly GetAllPagesQueryHandler _handler;
        public GetAllPagesQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IPageRepository>>();
            _handler = _fixture.Create<GetAllPagesQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Pages_Are_Returned()
        {
            // Arrange
            Guid sectionId = Guid.NewGuid();

            var query = new GetAllPagesQuery(sectionId);
            var response = new List<Page>()
            {
                new Page()
                {
                    Id = Guid.NewGuid(),
                    SectionId = sectionId,
                    Title = " ",
                    Key = Guid.NewGuid(),
                    Order = 1
                }
            };

            _repositoryMock.Setup(x => x.GetPagesForSectionAsync(sectionId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPagesForSectionAsync(sectionId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Data.Count);
            Assert.Single(result.Value.Data);
            Assert.Equal(response[0].Id, result.Value.Data[0].Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid sectionId = Guid.NewGuid();

            var query = new GetAllPagesQuery(sectionId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetPagesForSectionAsync(sectionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPagesForSectionAsync(sectionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}