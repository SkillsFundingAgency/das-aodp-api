using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Application.Queries.Application.Message;
using SFA.DAS.AODP.Models.Application;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Application.Tests.Queries.Application.Message
{
    public class GetApplicationMessagesByIdQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IApplicationMessagesRepository> _repositoryMock;
        private readonly GetApplicationMessagesByIdQueryHandler _handler;
        public GetApplicationMessagesByIdQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IApplicationMessagesRepository>>();
            _handler = _fixture.Create<GetApplicationMessagesByIdQueryHandler>();
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_ApplicationMessages_Are_Returned()
        {
            // Arrange
            UserType userType = 0;

            string userTypeString = userType.ToString();

            Guid applicationId = Guid.NewGuid();

            var query = new GetApplicationMessagesByIdQuery(applicationId, userTypeString);
            var response = new List<Data.Entities.Application.Message>()
            {
                new Data.Entities.Application.Message()
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = applicationId,
                    Text = " ",
                    Type = MessageType.UnlockApplication,
                    MessageHeader = " ", 
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = true,
                    SentByName = " ",
                    SentByEmail = " ",
                    SentAt = DateTime.UtcNow
                }
            };

            _repositoryMock.Setup(x => x.GetMessagesByApplicationIdAndUserTypeAsync(applicationId, userType))
                           .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetMessagesByApplicationIdAndUserTypeAsync(applicationId, userType), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(response.Count, result.Value.Messages.Count);
            Assert.Single(result.Value.Messages);
            Assert.Equal(response[0].ApplicationId, result.Value.Messages[0].ApplicationId);
        }

        [Fact]
        public async Task Then_The_Api_Is_Called_With_The_Request_And_Exception_Is_Handled()
        {
            // Arrange
            UserType userType = 0;

            string userTypeString = userType.ToString();

            Guid applicationId = Guid.NewGuid();

            var query = new GetApplicationMessagesByIdQuery(applicationId, userTypeString);

            Exception ex = new Exception();

            _repositoryMock.Setup(x => x.GetMessagesByApplicationIdAndUserTypeAsync(applicationId, userType))
                           .ThrowsAsync(ex);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(x => x.GetMessagesByApplicationIdAndUserTypeAsync(applicationId, userType), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(ex.Message, result.ErrorMessage);
        }
    }
}