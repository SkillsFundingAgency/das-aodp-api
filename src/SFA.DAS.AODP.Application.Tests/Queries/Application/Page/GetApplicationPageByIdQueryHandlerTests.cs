using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Page
{
    public class GetApplicationPageByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IPageRepository> _repositoryMock;
        private readonly GetApplicationPageByIdQueryHandler _handler;
        public GetApplicationPageByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IPageRepository>>();
            _handler = _fixture.Create<GetApplicationPageByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationPage_Is_Returned()
        {
            // Arrange
            int pageOrder = 1;

            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            var query = new GetApplicationPageByIdQuery(pageOrder, sectionId, formVersionId);
            var response = new Data.Entities.FormBuilder.Page()
            {
                Id = formVersionId,
                SectionId = sectionId,
                Order = 0,
                Questions = new()
                {
                    new()
                },
                Section = new()
                {
                    View_SectionPageCount = new()
                    {
                        PageCount = 1
                    }
                }
            };

            _repositoryMock.Setup(x => x.GetPageForApplicationAsync(pageOrder, sectionId, formVersionId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPageForApplicationAsync(pageOrder, sectionId, formVersionId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Id, result.Value.Id);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            int pageOrder = 1;

            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            var query = new GetApplicationPageByIdQuery(pageOrder, sectionId, formVersionId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetPageForApplicationAsync(pageOrder, sectionId, formVersionId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPageForApplicationAsync(pageOrder, sectionId, formVersionId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}