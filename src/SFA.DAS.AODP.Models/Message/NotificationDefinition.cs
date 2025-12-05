public class NotificationDefinition
{
    /// <summary>
    /// 
    /// </summary>
    public string TemplateName { get; set; } = default!;

    /// <summary>
    /// How the recipient should be resolved.
    /// </summary>
    public NotificationRecipientKind RecipientKind { get; set; }

    /// <summary>
    /// When RecipientKind == DirectEmail, this MUST be populated by the inner API.
    /// When RecipientKind is a system mailbox (QfauMailbox, etc.) this should be null.
    /// </summary>
    public string? EmailAddress { get; set; }

    /// <summary>
    /// Template tokens the outer API just passes straight through to the notification service.
    /// </summary>
    public Dictionary<string, string> EmailTokens { get; set; } = new();
}

public enum NotificationRecipientKind
{
    // Outer API uses config to resolve these to actual addresses:
    QfauMailbox,
    OfqualMailbox,
    SkillsEnglandMailbox,

    // Inner API must provide EmailAddress:
    DirectEmail
}

public static class EmailTemplateNames
{
    public const string QFASTApplicationMessageSentNotification = nameof(QFASTApplicationMessageSentNotification);
    public const string QFASTApplicationSubmittedNotification = nameof(QFASTApplicationSubmittedNotification);
    public const string QFASTSubmittedApplicationChangedNotification = nameof(QFASTSubmittedApplicationChangedNotification);
    public const string QFASTApplicationWithdrawnNotification = nameof(QFASTApplicationWithdrawnNotification);
}

