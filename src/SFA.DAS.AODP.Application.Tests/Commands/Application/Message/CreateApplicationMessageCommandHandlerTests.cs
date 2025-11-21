using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Message;
using SFA.DAS.AODP.Application.Services;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Models.Application;
using ApplicationMessage = SFA.DAS.AODP.Data.Entities.Application.Message;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Message
{
    public class CreateApplicationMessageCommandHandlerTests
    {
        private static readonly Guid ApplicationId = Guid.NewGuid();
        private static readonly Guid MessageId = Guid.NewGuid();

        private const string ValidUserType = nameof(UserType.AwardingOrganisation);             
        private const string ValidMessageType = nameof(MessageType.ReplyToInformationRequest);

        private readonly Mock<IApplicationReviewRepository> _reviewRepository = new();
        private readonly Mock<IApplicationReviewFeedbackRepository> _feedbackRepository = new();
        private readonly Mock<IApplicationMessagesRepository> _messageRepository = new();
        private readonly Mock<IApplicationRepository> _applicationRepository = new();
        private readonly Mock<INotificationDefinitionFactory> _notificationDefinitionFactory = new();

        private readonly CreateApplicationMessageCommandHandler _handler;

        public CreateApplicationMessageCommandHandlerTests()
        {
            _handler = new CreateApplicationMessageCommandHandler(
                _messageRepository.Object,
                _applicationRepository.Object,
                _feedbackRepository.Object,
                _reviewRepository.Object,
                _notificationDefinitionFactory.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesMessage_UpdatesApplication_AddsNotifications()
        {
            // Arrange
            var now = DateTime.UtcNow;

            var application = new Data.Entities.Application.Application
            {
                Id = ApplicationId,
                Status = ApplicationStatus.InReview.ToString(),
                UpdatedAt = now.AddMinutes(-10),
                NewMessage = false
            };

            _applicationRepository
                .Setup(r => r.GetByIdAsync(ApplicationId))
                .ReturnsAsync(application);

            _messageRepository
                .Setup(r => r.CreateAsync(It.IsAny<Data.Entities.Application.Message>()))
                .ReturnsAsync(MessageId);

            _reviewRepository
                .Setup(r => r.GetByApplicationIdAsync(ApplicationId))
                .ReturnsAsync((ApplicationReview?)null); 

            _feedbackRepository
                .Setup(r => r.UpdateAsync(It.IsAny<List<ApplicationReviewFeedback>>()))
                .Returns(Task.CompletedTask);

            var notificationDefinitions = new List<NotificationDefinition>
            {
                new()
                {
                    TemplateName = EmailTemplateNames.QFASTApplicationMessageSentNotification,
                    RecipientKind = NotificationRecipientKind.QfauMailbox
                }
            };

            _notificationDefinitionFactory
                .Setup(f => f.BuildForMessage(ApplicationId, MessageType.ReplyToInformationRequest))
                .ReturnsAsync(notificationDefinitions);

            var request = new CreateApplicationMessageCommand
            {
                ApplicationId = ApplicationId,
                UserType = ValidUserType,
                MessageType = ValidMessageType,
                MessageText = "Test message",
                SentByName = "Test User",
                SentByEmail = "test.user@test.com"
            };

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(MessageId, result.Value.Id);
                Assert.NotNull(result.Value.Notifications);
                Assert.Equal(notificationDefinitions.Count, result.Value.Notifications.Count);

                _messageRepository.Verify(
                    r => r.CreateAsync(It.Is<Data.Entities.Application.Message>(m =>
                        m.ApplicationId == ApplicationId &&
                        m.Text == request.MessageText &&
                        m.Type == MessageType.ReplyToInformationRequest &&
                        m.SentByName == request.SentByName &&
                        m.SentByEmail == request.SentByEmail &&
                        m.MessageHeader == MessageTypeConfigurationRules
                            .GetMessageSharingSettings(MessageType.ReplyToInformationRequest)
                            .MessageHeader)),
                    Times.Once);

                _applicationRepository.Verify(
                    r => r.GetByIdAsync(ApplicationId),
                    Times.Once);

                _applicationRepository.Verify(
                    r => r.UpdateAsync(It.Is<Data.Entities.Application.Application>(a =>
                        a.Id == ApplicationId &&
                        a.UpdatedAt > now)), 
                    Times.Once);

                _notificationDefinitionFactory.Verify(
                    f => f.BuildForMessage(ApplicationId, MessageType.ReplyToInformationRequest),
                    Times.Once);
            });
        }

        [Fact]
        public async Task Handle_InvalidUserType_ReturnsError_DoesNotCallRepositoriesOrFactory()
        {
            // Arrange
            var request = new CreateApplicationMessageCommand
            {
                ApplicationId = ApplicationId,
                UserType = "NotARealUserType",
                MessageType = ValidMessageType,
                MessageText = "Test message"
            };

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.False(result.Success);
                Assert.IsType<ArgumentException>(result.InnerException);
                Assert.Contains("Invalid User Type", result.ErrorMessage);

                _applicationRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
                _messageRepository.Verify(r => r.CreateAsync(It.IsAny<ApplicationMessage>()), Times.Never);
                _notificationDefinitionFactory.Verify(
                    f => f.BuildForMessage(It.IsAny<Guid>(), It.IsAny<MessageType>()),
                    Times.Never);
            });
        }

        [Fact]
        public async Task Handle_UserNotAllowedForMessageType_ReturnsError_DoesNotCreateMessage()
        {
            // Arrange
            var request = new CreateApplicationMessageCommand
            {
                ApplicationId = ApplicationId,
                UserType = UserType.Ofqual.ToString(),
                MessageType = ValidMessageType,
                MessageText = "Test message"
            };

            // Act
            var result = await _handler.Handle(request, default);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.NotNull(result);
                Assert.False(result.Success);
                Assert.IsType<ArgumentException>(result.InnerException);
                Assert.Contains("cannot create message type", result.ErrorMessage);

                _applicationRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
                _messageRepository.Verify(r => r.CreateAsync(It.IsAny<ApplicationMessage>()), Times.Never);
                _notificationDefinitionFactory.Verify(
                    f => f.BuildForMessage(It.IsAny<Guid>(), It.IsAny<MessageType>()),
                    Times.Never);
            });
        }
    }
}
