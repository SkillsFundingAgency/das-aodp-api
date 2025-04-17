using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities.Application;

public class Message
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string Text { get; set; }
    [Column(TypeName = "nvarchar(4000)")]
    public MessageType Type { get; set; }
    public string MessageHeader { get; set; }

    public bool SharedWithDfe { get; set; }
    public bool SharedWithOfqual { get; set; }
    public bool SharedWithSkillsEngland { get; set; }
    public bool SharedWithAwardingOrganisation { get; set; }
    public string SentByName { get; set; }
    public string SentByEmail { get; set; }
    public DateTime SentAt { get; set; }
}