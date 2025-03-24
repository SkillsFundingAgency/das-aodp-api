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

    ApplicationSubmitted,

    OfqualFeedbackSubmitted,
    SkillsEnglandFeedbackSubmitted,

    QfauOwnerUpdated,
    SkillsEnglandOwnerUpdated,
    OfqualOwnerUpdated,

    QanUpdated,

    AoInformedOfDecision
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
                    AvailableTo = [UserType.Qfau]
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
                    AvailableTo = [UserType.Qfau]
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
                    AvailableTo = [UserType.Qfau]
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
                    AvailableTo = [UserType.Ofqual]
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
                    AvailableTo = [UserType.SkillsEngland]
                }
            },

            {
                MessageType.InternalNotes,  new MessageTypeConfiguration
                {
                    MessageHeader = "Internal note by DfE",
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
                    MessageHeader = "Internal note for DfE by Ofqual",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = false,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.Ofqual]
                }
            },

            {
                MessageType.InternalNotesForQfauFromSkillsEngland,  new MessageTypeConfiguration
                {
                    MessageHeader = "Internal note for DfE by Skills England",
                    SharedWithDfe = true,
                    SharedWithOfqual = false,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.SkillsEngland]
                }
            },

            {
                MessageType.InternalNotesForPartners,  new MessageTypeConfiguration
                {
                    MessageHeader = "Internal note for partners by DfE",
                    SharedWithDfe = true,
                    SharedWithOfqual = true,
                    SharedWithSkillsEngland = true,
                    SharedWithAwardingOrganisation = false,
                    AvailableTo = [UserType.Qfau]
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
                    AvailableTo = [UserType.AwardingOrganisation]
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


            {
                MessageType.ApplicationSubmitted,
                 new MessageTypeConfiguration
                 {
                        MessageHeader = "Application submitted",
                        SharedWithDfe = true,
                        SharedWithOfqual = true,
                        SharedWithSkillsEngland = true,
                        SharedWithAwardingOrganisation = true,
                        AvailableTo = [UserType.AwardingOrganisation]
                 }
            },

            {
                MessageType.OfqualFeedbackSubmitted,
                 new MessageTypeConfiguration
                 {
                        MessageHeader = "Ofqual feedback",
                        SharedWithDfe = true,
                        SharedWithOfqual = true,
                        SharedWithSkillsEngland = false,
                        SharedWithAwardingOrganisation = false,
                        AvailableTo = [UserType.Ofqual]
                 }
            },
            {
                MessageType.SkillsEnglandFeedbackSubmitted,
                 new MessageTypeConfiguration
                 {
                        MessageHeader = "Skills England feedback",
                        SharedWithDfe = true,
                        SharedWithOfqual = false,
                        SharedWithSkillsEngland = true,
                        SharedWithAwardingOrganisation = false,
                        AvailableTo = [UserType.SkillsEngland]
                 }
            },
            {
                MessageType.QfauOwnerUpdated,
                 new MessageTypeConfiguration
                 {
                        MessageHeader = "DfE owner updated",
                        SharedWithDfe = true,
                        SharedWithOfqual = false,
                        SharedWithSkillsEngland = false,
                        SharedWithAwardingOrganisation = false,
                        AvailableTo = [UserType.Qfau]
                 }
            },
            {
                MessageType.SkillsEnglandOwnerUpdated,
                 new MessageTypeConfiguration
                 {
                        MessageHeader = "Skills England owner updated",
                        SharedWithDfe = true,
                        SharedWithOfqual = false,
                        SharedWithSkillsEngland = true,
                        SharedWithAwardingOrganisation = false,
                        AvailableTo = [UserType.SkillsEngland]
                 }
            },
            {
                MessageType.OfqualOwnerUpdated,
                 new MessageTypeConfiguration
                 {
                        MessageHeader = "Ofqual owner updated",
                        SharedWithDfe = true,
                        SharedWithOfqual = true,
                        SharedWithSkillsEngland = false,
                        SharedWithAwardingOrganisation = false,
                        AvailableTo = [UserType.Ofqual]
                 }
            },
            {
                MessageType.QanUpdated,
                 new MessageTypeConfiguration
                 {
                        MessageHeader = "QAN updated by DfE",
                        SharedWithDfe = true,
                        SharedWithOfqual = true,
                        SharedWithSkillsEngland = true,
                        SharedWithAwardingOrganisation = true,
                        AvailableTo = [UserType.Qfau]
                 }
            },
              {
                MessageType.AoInformedOfDecision,
                 new MessageTypeConfiguration
                 {
                        MessageHeader = "Awarding Organisation informed of funding decision",
                        SharedWithDfe = true,
                        SharedWithOfqual = true,
                        SharedWithSkillsEngland = true,
                        SharedWithAwardingOrganisation = true,
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