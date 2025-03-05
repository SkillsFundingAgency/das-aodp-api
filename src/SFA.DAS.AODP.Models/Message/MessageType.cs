using SFA.DAS.AODP.Models.Application;

public enum MessageType
{
    UnlockApplication, // DfE only
    PutApplicationOnHold, // DfE only

    RequestInformationFromAOByQfau,
    RequestInformationFromAOByOfqaul,
    RequestInformationFromAOBySkillsEngland,

    ReplyToInformationRequest, // AO only

    InternalNotes, // DfE to DfE

    InternalNotesForQfauFromOfqual,
    InternalNotesForQfauFromSkillsEngland,

    InternalNotesForPartners
}

public static class MessageTypeConfigurationRules
{
    public static readonly Dictionary<MessageType, Func<MessageTypeConfiguration>> MessageTypeConfigurations =
        new()
        {
            // DfE
            { MessageType.UnlockApplication, () => new MessageTypeConfiguration
            {
                DisplayName = "Unlock Application", // only for the UI - textual representaton
                MessageHeader = "Application Unlocked",
                SharedWithDfe = true,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = false, 
                SharedWithAwardingOrganisation = true } },

            { MessageType.PutApplicationOnHold, () => new MessageTypeConfiguration
            {
                DisplayName = "Put Application On Hold",
                MessageHeader = "Application Put On Hold",
                SharedWithDfe = true,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = false,
                SharedWithAwardingOrganisation = true } },

            { MessageType.RequestInformationFromAOByQfau, () => new MessageTypeConfiguration
            {
                DisplayName = "Request Information From AO",
                MessageHeader = "Information Requested From Awarding Organisation",
                SharedWithDfe = true,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = false,
                SharedWithAwardingOrganisation = true } },

            { MessageType.RequestInformationFromAOByOfqaul, () => new MessageTypeConfiguration
            {
                DisplayName = "Request Information From AO",
                MessageHeader = "Information Requested From Awarding Organisation",
                SharedWithDfe = false,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = false,
                SharedWithAwardingOrganisation = true } },

            { MessageType.RequestInformationFromAOBySkillsEngland, () => new MessageTypeConfiguration
            {
                DisplayName = "Request Information From AO",
                MessageHeader = "Information Requested From Awarding Organisation",
                SharedWithDfe = false,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = true } },

            { MessageType.InternalNotes, () => new MessageTypeConfiguration
            {
                DisplayName = "Internal Notes",
                MessageHeader = "Internal Note",
                SharedWithDfe = true,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = false,
                SharedWithAwardingOrganisation = false } },

            { MessageType.InternalNotesForQfauFromOfqual, () => new MessageTypeConfiguration
            {
                DisplayName = "Internal Notes for DfE",
                MessageHeader = "Internal Note for DfE",
                SharedWithDfe = true,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = false,
                SharedWithAwardingOrganisation = false } },
            
            { MessageType.InternalNotesForQfauFromSkillsEngland, () => new MessageTypeConfiguration
            {
                DisplayName = "Internal Notes for DfE",
                MessageHeader = "Internal Note for DfE",
                SharedWithDfe = true,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = false } },

            { MessageType.InternalNotesForPartners, () => new MessageTypeConfiguration
            {
                DisplayName = "Internal Notes For Partners",
                MessageHeader = "Internal Note for Partners",
                SharedWithDfe = true,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = false } },

            { MessageType.ReplyToInformationRequest, () => new MessageTypeConfiguration
            {
                DisplayName = "Reply To Information Request",
                MessageHeader = "Answer to Information Request from Awarding Organisation",
                SharedWithDfe = true,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = true } }
        };

    public static MessageTypeConfiguration GetMessageSharingSettings(MessageType messageType)
    {
        if (MessageTypeConfigurations.TryGetValue(messageType, out var config))
        {
            return config();
        }

        return new MessageTypeConfiguration { };
    }
}

public class MessageTypeConfiguration
{
    public string DisplayName { get; set; }
    public string MessageHeader { get; set; }
    public bool SharedWithDfe { get; set; }
    public bool SharedWithAwardingOrganisation { get; set; }
    public bool SharedWithSkillsEngland { get; set; }
    public bool SharedWithOfqual { get; set; }
}