using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Data.Entities.Feedback;
using SFA.DAS.AODP.Data.Repositories.Feedback;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Feedback
{
    public class SaveSurveyCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ISurveyRepository> _surveyRepositoryMock;
        private readonly SaveSurveyCommandHandler _handler;

        public SaveSurveyCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _surveyRepositoryMock = _fixture.Freeze<Mock<ISurveyRepository>>();
            _handler = new SaveSurveyCommandHandler(_surveyRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenSurveyIsSaved()
        {
            // Arrange
            var command = _fixture.Create<SaveSurveyCommand>();

            _surveyRepositoryMock.Setup(repo => repo.Create(It.IsAny<Survey>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _surveyRepositoryMock.Verify(repo => repo.Create(It.Is<Survey>(s =>
                s.Page == command.Page &&
                s.SatisfactionScore == command.SatisfactionScore &&
                s.Comments == command.Comments
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var command = _fixture.Create<SaveSurveyCommand>();
            var exception = new Exception("Test exception");

            _surveyRepositoryMock.Setup(repo => repo.Create(It.IsAny<Survey>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(exception.Message, result.ErrorMessage);
            Assert.Equal(exception, result.InnerException);
        }
    }
}
