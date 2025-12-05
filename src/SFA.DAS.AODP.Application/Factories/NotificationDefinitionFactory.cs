using SFA.DAS.AODP.Data.Repositories.Application;

namespace SFA.DAS.AODP.Application.Services
{
    public interface INotificationDefinitionFactory
    {
        Task<List<NotificationDefinition>> BuildForMessage(
            Guid applicationId,
            MessageType messageType);
    }

    public class NotificationDefinitionFactory : INotificationDefinitionFactory
    {
        private readonly IApplicationMessagesRepository _messageRepository;

        public NotificationDefinitionFactory(IApplicationMessagesRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<List<NotificationDefinition>> BuildForMessage(
            Guid applicationId,
            MessageType messageType)
        {
            var notifications = new List<NotificationDefinition>();

            switch (messageType)
            {
                case MessageType.AoInformedOfDecision:
                case MessageType.UnlockApplication:
                case MessageType.RequestInformationFromAOBySkillsEngland:
                case MessageType.RequestInformationFromAOByOfqaul:
                case MessageType.RequestInformationFromAOByQfau:
                    string? aoEmail = await GetAoEmailForApplication(applicationId);

                    if (!string.IsNullOrEmpty(aoEmail))
                    {
                        notifications.Add(new NotificationDefinition
                        {
                            TemplateName = EmailTemplateNames.QFASTApplicationMessageSentNotification,
                            RecipientKind = NotificationRecipientKind.DirectEmail,
                            EmailAddress = aoEmail,
                        });
                    }
                    break;
                case MessageType.ApplicationSubmitted:
                    notifications.Add(new NotificationDefinition
                    {
                        TemplateName = EmailTemplateNames.QFASTApplicationSubmittedNotification,
                        RecipientKind = NotificationRecipientKind.QfauMailbox,
                    });
                    break;
                case MessageType.ReplyToInformationRequest:
                    notifications.Add(new NotificationDefinition
                    {
                        TemplateName = EmailTemplateNames.QFASTApplicationMessageSentNotification,
                        RecipientKind = NotificationRecipientKind.QfauMailbox,
                    });
                    break;

                case MessageType.OfqualFeedbackSubmitted:
                case MessageType.SkillsEnglandFeedbackSubmitted:
                    notifications.Add(new NotificationDefinition
                    {
                        TemplateName = EmailTemplateNames.QFASTSubmittedApplicationChangedNotification,
                        RecipientKind = NotificationRecipientKind.QfauMailbox,
                    });
                    break;
                default:
                    break;
            }
            return notifications;
        }

        private async Task<string?> GetAoEmailForApplication(Guid applicationId)
        {
            var submissionMessage = await _messageRepository
                .GetLatestByTypeAsync(applicationId, MessageType.ApplicationSubmitted);

            return submissionMessage?.SentByEmail;
        }
    }

}
