using Moq;
using SFA.DAS.AODP.Application.Services;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using static SFA.DAS.AODP.Application.Queries.Application.Message.GetApplicationMessagesByApplicationIdQueryResponse;

namespace SFA.DAS.AODP.Application.Tests.Services
{
    public class NotificationDefinitionFactoryTests
    {

        private static readonly Guid ApplicationId = Guid.NewGuid();
        private const string AoEmail = "ao@test.com";

        private Mock<IApplicationMessagesRepository> _messageRepository = null!;
        private NotificationDefinitionFactory _factory = null!;

        public NotificationDefinitionFactoryTests()
        {
            _messageRepository = new Mock<IApplicationMessagesRepository>();
            _factory = new NotificationDefinitionFactory(_messageRepository.Object);
        }

        [Fact]
        public async Task BuildForMessage_AoMessageType_WithAoEmail_ReturnsDirectEmailNotification()
        {
            // Arrange
            var applicationSubmittedMessage = new Message
            {
                ApplicationId = ApplicationId,
                Type = MessageType.ApplicationSubmitted,
                SentByEmail = AoEmail
            };

            _messageRepository
                .Setup(r => r.GetLatestByTypeAsync(ApplicationId, MessageType.ApplicationSubmitted))
                .ReturnsAsync(applicationSubmittedMessage);

            var messageType = MessageType.AoInformedOfDecision;

            // Act
            var result = await _factory.BuildForMessage(ApplicationId, messageType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Single(result);

                var notification = result.Single();

                Assert.Equal(EmailTemplateNames.QFASTApplicationMessageSentNotification, notification.TemplateName);
                Assert.Equal(NotificationRecipientKind.DirectEmail, notification.RecipientKind);
                Assert.Equal(AoEmail, notification.EmailAddress);
            });
        }

        [Fact]
        public async Task BuildForMessage_AoMessageType_NoSubmittedMessage_ReturnsEmptyList()
        {
            // Arrange
            _messageRepository
                .Setup(r => r.GetLatestByTypeAsync(ApplicationId, MessageType.ApplicationSubmitted))
                .ReturnsAsync((Message?)null);

            var messageType = MessageType.AoInformedOfDecision;

            // Act
            var result = await _factory.BuildForMessage(ApplicationId, messageType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Empty(result);
            });
        }

        [Fact]
        public async Task BuildForMessage_ApplicationSubmitted_ReturnsQfauMailboxNotification()
        {
            // Arrange
            var messageType = MessageType.ApplicationSubmitted;

            // Act
            var result = await _factory.BuildForMessage(ApplicationId, messageType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Single(result);

                var notification = result.Single();

                Assert.Equal(EmailTemplateNames.QFASTApplicationSubmittedNotification, notification.TemplateName);
                Assert.Equal(NotificationRecipientKind.QfauMailbox, notification.RecipientKind);
                Assert.Null(notification.EmailAddress);
            });
        }

        [Fact]
        public async Task BuildForMessage_ReplyToInformationRequest_ReturnsQfauMailboxNotification()
        {
            // Arrange
            var messageType = MessageType.ReplyToInformationRequest;

            // Act
            var result = await _factory.BuildForMessage(ApplicationId, messageType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Single(result);

                var notification = result.Single();

                Assert.Equal(EmailTemplateNames.QFASTApplicationMessageSentNotification, notification.TemplateName);
                Assert.Equal(NotificationRecipientKind.QfauMailbox, notification.RecipientKind);
                Assert.Null (notification.EmailAddress);
            });
        }

        [Fact]
        public async Task BuildForMessage_OfqualFeedbackSubmitted_ReturnsSubmittedApplicationChangedNotification()
        {
            // Arrange
            var messageType = MessageType.OfqualFeedbackSubmitted;

            // Act
            var result = await _factory.BuildForMessage(ApplicationId, messageType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Single(result);

                var notification = result.Single();

                Assert.Equal(EmailTemplateNames.QFASTSubmittedApplicationChangedNotification, notification.TemplateName);
                Assert.Equal(NotificationRecipientKind.QfauMailbox, notification.RecipientKind);
                Assert.Null(notification.EmailAddress);
            });
        }

        [Fact]
        public async Task BuildForMessage_SkillsEnglandFeedbackSubmitted_ReturnsSubmittedApplicationChangedNotification()
        {
            // Arrange
            var messageType = MessageType.SkillsEnglandFeedbackSubmitted;

            // Act
            var result = await _factory.BuildForMessage(ApplicationId, messageType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Single(result);

                var notification = result.Single();

                Assert.Equal(EmailTemplateNames.QFASTSubmittedApplicationChangedNotification, notification.TemplateName);
                Assert.Equal(NotificationRecipientKind.QfauMailbox, notification.RecipientKind);
                Assert.Null(notification.EmailAddress);
            });
        }

        [Fact]
        public async Task BuildForMessage_UnhandledMessageType_ReturnsEmptyList()
        {
            // Arrange
            var messageType = MessageType.InternalNotes; 

            // Act
            var result = await _factory.BuildForMessage(ApplicationId, messageType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.Empty(result);
            });
        }
    }
}
