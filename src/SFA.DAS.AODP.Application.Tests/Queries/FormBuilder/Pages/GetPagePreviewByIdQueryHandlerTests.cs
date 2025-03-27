using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Forms;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Pages;
using SFA.DAS.AODP.Application.Exceptions;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Sections;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.FormBuilder.Pages
{
    public class GetPagePreviewByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IPageRepository> _repositoryMock;
        private readonly GetPagePreviewByIdQueryHandler _handler;
        public GetPagePreviewByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IPageRepository>>();
            _handler = _fixture.Create<GetPagePreviewByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_PagePreview_Is_Returned()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            Guid questionId = Guid.NewGuid();

            var query = new GetPagePreviewByIdQuery(pageId, sectionId, formVersionId);
            var response = new Page()
            {
                SectionId = sectionId,
                Id = pageId,
                Title = " ",
                Questions = new()
                {
                    new()
                    {
                        Id = questionId,
                        Title = " ",
                        Order = 1,
                        Required = false,
                        Type = " ",
                    }
                }
            };

            _repositoryMock.Setup(x => x.GetPageByIdAsync(pageId))
                .ReturnsAsync(response);

            _repositoryMock.Setup(x => x.GetPageForApplicationAsync(response.Order, sectionId, formVersionId))
              .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPageByIdAsync(pageId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Questions.Count, result.Value.Questions.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid formVersionId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            var query = new GetPagePreviewByIdQuery(pageId, sectionId, formVersionId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetPageByIdAsync(pageId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetPageByIdAsync(pageId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}