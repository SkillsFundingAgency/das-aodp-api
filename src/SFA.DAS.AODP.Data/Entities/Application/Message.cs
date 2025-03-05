namespace SFA.DAS.AODP.Data.Entities.Application;

public class Message
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string Text { get; set; }
    public string? Status { get; set; }
    public string Type { get; set; }

    public bool SharedWithDfe { get; set; }
    public bool SharedWithOfqual { get; set; }
    public bool SharedWithSkillsEngland { get; set; }
    public bool SharedWithAwardingOrganisation { get; set; }
    public string SentByName { get; set; }
    public string SentByEmail { get; set; }
    public DateTime SentAt { get; set; }
}

public enum MessageType
{
    UnlockApplication,
    PutApplicationOnHold,
    RequestInformation,
    RequestInformationFromAO,
    ReplyToInformationRequest,
    InternalNotes,
    InternalNotesForDfe,
    InternalNotesForPartners
}

// things we want to show: 
// 1. Message Status - InReview, Awaiting Response From AO
// 2. Application Status - Application Submitted, etc.