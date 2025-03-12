﻿using SFA.DAS.AODP.Models.Application;

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

    InternalNotesForPartners
}

public static class MessageTypeConfigurationRules
{
    public static readonly Dictionary<MessageType, Func<MessageTypeConfiguration>> MessageTypeConfigurations =
        new()
        {
    { MessageType.UnlockApplication, () => new MessageTypeConfiguration
            {
                DisplayName = "Unlock Application",
                MessageHeader = "Application Unlocked",
                SharedWithDfe = true,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = false,
                SharedWithAwardingOrganisation = true,
                AvailableTo = [UserType.Qfau, UserType.AwardingOrganisation]
            } },

            { MessageType.PutApplicationOnHold, () => new MessageTypeConfiguration
            {
                DisplayName = "Put Application On Hold",
                MessageHeader = "Application Put On Hold",
                SharedWithDfe = true,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = false,
                SharedWithAwardingOrganisation = true,
                AvailableTo = [UserType.Qfau, UserType.AwardingOrganisation]
            } },

            { MessageType.RequestInformationFromAOByQfau, () => new MessageTypeConfiguration
            {
                DisplayName = "Request Information From AO",
                MessageHeader = "Information Requested From Awarding Organisation",
                SharedWithDfe = true,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = true,
                AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland, UserType.AwardingOrganisation]
            } },

            { MessageType.RequestInformationFromAOByOfqaul, () => new MessageTypeConfiguration
            {
                DisplayName = "Request Information From AO",
                MessageHeader = "Information Requested From Awarding Organisation",
                SharedWithDfe = true,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = true,
                AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland, UserType.AwardingOrganisation]
            } },

            { MessageType.RequestInformationFromAOBySkillsEngland, () => new MessageTypeConfiguration
            {
                DisplayName = "Request Information From AO",
                MessageHeader = "Information Requested From Awarding Organisation",
                SharedWithDfe = true,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = true,
                AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland, UserType.AwardingOrganisation]
            } },

            { MessageType.InternalNotes, () => new MessageTypeConfiguration
            {
                DisplayName = "Internal Notes",
                MessageHeader = "Internal Note",
                SharedWithDfe = true,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = false,
                SharedWithAwardingOrganisation = false,
                AvailableTo = [UserType.Qfau]
            } },

            { MessageType.InternalNotesForQfauFromOfqual, () => new MessageTypeConfiguration
            {
                DisplayName = "Internal Notes for DfE",
                MessageHeader = "Internal Note for DfE",
                SharedWithDfe = true,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = false,
                SharedWithAwardingOrganisation = false,
                AvailableTo = [UserType.Qfau, UserType.Ofqual]
            } },

            { MessageType.InternalNotesForQfauFromSkillsEngland, () => new MessageTypeConfiguration
            {
                DisplayName = "Internal Notes for DfE",
                MessageHeader = "Internal Note for DfE",
                SharedWithDfe = true,
                SharedWithOfqual = false,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = false,
                AvailableTo = [UserType.Qfau, UserType.SkillsEngland]
            } },

            { MessageType.InternalNotesForPartners, () => new MessageTypeConfiguration
            {
                DisplayName = "Internal Notes for Partners",
                MessageHeader = "Internal Note for Partners",
                SharedWithDfe = true,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = false,
                AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland]
            } },

            { MessageType.ReplyToInformationRequest, () => new MessageTypeConfiguration
            {
                DisplayName = "Reply To Information Request",
                MessageHeader = "Answer to Information Request from Awarding Organisation",
                SharedWithDfe = true,
                SharedWithOfqual = true,
                SharedWithSkillsEngland = true,
                SharedWithAwardingOrganisation = true,
                AvailableTo = [UserType.Qfau, UserType.Ofqual, UserType.SkillsEngland, UserType.AwardingOrganisation]
            } }
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
    public List<UserType> AvailableTo { get; set; }
}