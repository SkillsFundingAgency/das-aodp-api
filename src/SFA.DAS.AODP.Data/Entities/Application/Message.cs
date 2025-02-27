using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AODP.Data.Entities.Application;

public class Message
{

    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; } // FK
    public string Text { get; set; }
    // status
    // message type

    public bool SharedWithDfe { get; set; }
    public bool SharedWithOfqual { get; set; }
    public bool SharedWithSkillsEngland { get; set; }
    public bool SharedWithAwardingOrganisation { get; set; }
    public string SentByEmail { get; set; }
    public DateTime SentAt { get; set; }
}