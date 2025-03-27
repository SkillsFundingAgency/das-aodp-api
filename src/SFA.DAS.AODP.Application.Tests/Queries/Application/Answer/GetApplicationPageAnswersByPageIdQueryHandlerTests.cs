using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Answer
{
    public class GetApplicationPageAnswersByPageIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationQuestionAnswerRepository> _repositoryMock;
        private readonly GetApplicationPageAnswersByPageIdQueryHandler _handler;
        public GetApplicationPageAnswersByPageIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationQuestionAnswerRepository>>();
            _handler = _fixture.Create<GetApplicationPageAnswersByPageIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationQuestionAnswer_Is_Returned()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            var query = new GetApplicationPageAnswersByPageIdQuery(applicationId, pageId, sectionId, formVersionId);
            var response = new List<ApplicationQuestionAnswer> 
            {
                new ApplicationQuestionAnswer()
                {
                    Id = applicationId,
                    ApplicationPageId = pageId
                }
            };

            _repositoryMock.Setup(x => x.GetAnswersByApplicationAndPageId(applicationId, pageId))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAnswersByApplicationAndPageId(applicationId, pageId), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Questions.Count);
            Assert.Single(result.Value.Questions);
            Assert.Equal(response[0].QuestionId, result.Value.Questions[0].QuestionId);
        }


        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            Guid applicationId = Guid.NewGuid();

            Guid pageId = Guid.NewGuid();

            Guid sectionId = Guid.NewGuid();

            Guid formVersionId = Guid.NewGuid();

            var query = new GetApplicationPageAnswersByPageIdQuery(applicationId, pageId, sectionId, formVersionId);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetAnswersByApplicationAndPageId(applicationId, pageId))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetAnswersByApplicationAndPageId(applicationId, pageId), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}