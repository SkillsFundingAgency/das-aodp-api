using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Application.Queries.FormBuilder.Routes;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Application.Exceptions;

namespace SFA.DAS.AODP.Application.Tests.Queries.FormBuilder.Routes
{
    public class GetAvailableQuestionsForRoutingQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IRouteRepository> _repositoryMock;
        private readonly GetAvailableQuestionsForRoutingQueryHandler _handler;
        public GetAvailableQuestionsForRoutingQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IRouteRepository>>();
            _handler = _fixture.Create<GetAvailableQuestionsForRoutingQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_AvailableQuestions_Are_Returned()
        {
            // Arrange
            Guid pageId = Guid.NewGuid();

            var query = new GetAvailableQuestionsForRoutingQuery();
            query.PageId = pageId;
            var response = new List<View_AvailableQuestionsForRouting>()
            {
                new()
            };

            _repositoryMock.Setup(x => x.GetAvailableQuestionsForRoutingByPageId(pageId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAvailableQuestionsForRoutingByPageId(pageId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Questions.Count);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_RecordNotFoundException_Is_Handled()
        {
            // Arrange
            Guid pageId = Guid.NewGuid();

            RecordNotFoundException ex = new RecordNotFoundException(pageId);

            NotFoundException expected = new NotFoundException(pageId);

            var query = new GetAvailableQuestionsForRoutingQuery();
            query.PageId = pageId;
            var response = new List<View_AvailableQuestionsForRouting>()
            {
                new()
            };

            _repositoryMock.Setup(x => x.GetAvailableQuestionsForRoutingByPageId(pageId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAvailableQuestionsForRoutingByPageId(pageId), Times.Once);
            Assert.False(result.Success);
            Assert.IsType<NotFoundException>(result.InnerException);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid pageId = Guid.NewGuid();

            Exception ex = new Exception();

            var query = new GetAvailableQuestionsForRoutingQuery();
            query.PageId = pageId;
            var response = new List<View_AvailableQuestionsForRouting>()
            {
                new()
            };

            _repositoryMock.Setup(x => x.GetAvailableQuestionsForRoutingByPageId(pageId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAvailableQuestionsForRoutingByPageId(pageId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}