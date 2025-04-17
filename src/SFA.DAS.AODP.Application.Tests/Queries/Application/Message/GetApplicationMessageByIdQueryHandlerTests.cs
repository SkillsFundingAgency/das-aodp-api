using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Application.Message;
using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Application.Message
{
    public class GetApplicationMessageByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationMessagesRepository> _repositoryMock;
        private readonly GetApplicationMessageByIdQueryHandler _handler;
        public GetApplicationMessageByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationMessagesRepository>>();
            _handler = _fixture.Create<GetApplicationMessageByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Message_Is_Returned()
        {
            // Arrange
            var query = _fixture.Create<GetApplicationMessageByIdQuery>();
            var response = new Data.Entities.Application.Message()
            {
                Id = Guid.NewGuid(),
            };

            _repositoryMock.Setup(x => x.GetByIdAsync(query.MessageId)).ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetByIdAsync(query.MessageId), Times.Once());
            Assert.True(result.Success);
            Assert.Equal(response.Id, result.Value.MessageId);
        }


        [Fact]
        public async Task Then_Exception_Thrown_And_Handled()
        {
            // Arrange
            var query = _fixture.Create<GetApplicationMessageByIdQuery>();
            var exceptionMessage = "An error occurred";
            _repositoryMock.Setup(x => x.GetByIdAsync(query.MessageId))
                           .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetByIdAsync(query.MessageId), Times.Once());
            Assert.False(result.Success);
            Assert.Equal(exceptionMessage, result.ErrorMessage);
        }
    }
}