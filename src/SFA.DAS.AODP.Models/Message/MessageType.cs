using SFA.DAS.AODP.Models.Application;

public enum MessageType
{
    UnlockApplication,
    PutApplicationOnHold,

    RequestInformationFromAOByQfau,
    RequestInformationFromAOByOfqaul,
    RequestInformationFromAOBySkillsEngland,

    ReplyToInformationRequest,

    InternalNotes,

    InternalNotesForQfauFromOfqual,
    InternalNotesForQfauFromSkillsEngland,

    InternalNotesForPartners,

    // System Audit
    ApplicationSharedWithOfqual,
    ApplicationSharedWithSkillsEngland,

    ApplicationUnsharedWithOfqual,
    ApplicationUnsharedWithSkillsEngland,
}

public static class MessageTypeConfigurationRules
{
    public static readonly Dictionary<MessageType, MessageTypeConfiguration> MessageTypeConfigurations =
        new()
        {
            {
                MessageType.UnlockApplication,  new MessageTypeConfiguration
                {
                    MessageHeader = "Application unlocked",
                    SharedWithDfe = true,
                    SharedWithOfqual = false,
                    SharedWithSkillsEngland = false,
                    SharedWithAwardingOrganisation = true,
                    AvailableTo = [UserType.Qfau, UserType.AwardingOrganisation]
                }
            },

            {
                MessageType.PutApplicationOnHold,  new MessageTypeConfiguration
                {
                    MessageHeader = "Application put on hold",
                    SharedWithDfe = true,
                    SharedWithOfqual = false,
                    SharedWithSkillsEngland = false,
                    SharedWithAwardingOrganisation = true,
                    AvailableTo = [UserType.Qfau, UserType.AwardingOrganisation]
                }
            },

            {
                MessageType.RequestInformationFromAOByQfau,  new MessageTypeConfiguration
                {
                    MessageHeader = "Information requested from Awarding Organisation by DfE",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = true,
                    AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland, UserType.AwardingOrganisation]
                }
            },

            {
                MessageType.RequestInformationFromAOByOfqaul,  new MessageTypeConfiguration
                {
                    MessageHeader = "Information requested from Awarding Organisation by Ofqual",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = true,
                    AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland, UserType.AwardingOrganisation]
                }
            },

            {
                MessageType.RequestInformationFromAOBySkillsEngland,  new MessageTypeConfiguration
                {
                    MessageHeader = "Information requested from Awarding Organisation by Skills England",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = true,
                    AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland, UserType.AwardingOrganisation]
                }
            },

            {
                MessageType.InternalNotes,  new MessageTypeConfiguration
                {
                    MessageHeader = "Internal note",
                    SharedWithDfe = true,
                    SharedWithOfqual = false,
                    SharedWithSkillsEngland = false,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.Qfau]
                }
            },

            {
                MessageType.InternalNotesForQfauFromOfqual,  new MessageTypeConfiguration
                {
                    MessageHeader = "Internal note for DfE",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = false,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.Qfau, UserType.Ofqual]
                }
            },

            {
                MessageType.InternalNotesForQfauFromSkillsEngland,  new MessageTypeConfiguration
                {
                    MessageHeader = "Internal note for DfE",
                    SharedWithDfe = true,
                    SharedWithOfqual = false,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.Qfau, UserType.SkillsEngland]
                }
            },

            {
                MessageType.InternalNotesForPartners,  new MessageTypeConfiguration
                {
                    MessageHeader = "Internal note for partners",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland]
                }
            },

            {
                MessageType.ReplyToInformationRequest,  new MessageTypeConfiguration
                {
                    MessageHeader = "Answer to information request from Awarding Organisation",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = true,
                    AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland, UserType.AwardingOrganisation]
                }
            },

            // System Audit Messages
            {
                MessageType.ApplicationSharedWithOfqual,
                 new MessageTypeConfiguration
                {
                    MessageHeader = "Application shared with Ofqual",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = false,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.Qfau]
                }
            },

            {
                MessageType.ApplicationSharedWithSkillsEngland,
                 new MessageTypeConfiguration
                {
                    MessageHeader = "Application shared with Skills England",
                    SharedWithDfe = true,
                    SharedWithOfqual = false,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.Qfau]
                }
            },

            {
                MessageType.ApplicationUnsharedWithOfqual,
                 new MessageTypeConfiguration
                {
                    MessageHeader = "Application unshared with Ofqual",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = false,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.Qfau]
                }
            },

            {
                MessageType.ApplicationUnsharedWithSkillsEngland,
                 new MessageTypeConfiguration
                 {
                        MessageHeader = "Application unshared with Skills England",
                        SharedWithDfe = true,
                        SharedWithOfqual = false,
                        SharedWithSkillsEngland = true,
                        SharedWithAwardingOrganisation = false,
                        AvailableTo = [UserType.Qfau]
                 }
            },
        };

    public static MessageTypeConfiguration GetMessageSharingSettings(MessageType messageType)
    {
        if (MessageTypeConfigurations.TryGetValue(messageType, out var config))
        {
            return config;
        }

        return new MessageTypeConfiguration { };
    }
}

public class MessageTypeConfiguration
{
    public string MessageHeader { get; set; }
    public bool SharedWithDfe { get; set; }
    public bool SharedWithAwardingOrganisation { get; set; }
    public bool SharedWithSkillsEngland { get; set; }
    public bool SharedWithOfqual { get; set; }
    public List<UserType> AvailableTo { get; set; }
}