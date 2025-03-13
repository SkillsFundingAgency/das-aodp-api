namespace SFA.DAS.AODP.Application.Queries.Application.Message;

public class GetApplicationMessagesByIdQueryResponse
{
    public List<ApplicationMessage> Messages { get; set; } = new();
    public class ApplicationMessage
    {
        public Guid MessageId { get; set; }
        public Guid ApplicationId { get; set; }
        public string MessageText { get; set; }
        public string MessageType { get; set; }
        public string MessageHeader { get; set; }
        public bool SharedWithDfe { get; set; }
        public bool SharedWithOfqual { get; set; }
        public bool SharedWithSkillsEngland { get; set; }
        public bool SharedWithAwardingOrganisation { get; set; }
        public string SentByName { get; set; }
        public string SentByEmail { get; set; }
        public DateTime SentAt { get; set; }
    }

    public static implicit operator GetApplicationMessagesByIdQueryResponse(List<SFA.DAS.AODP.Data.Entities.Application.Message> messages)
    {
        GetApplicationMessagesByIdQueryResponse response = new();

        foreach (var message in messages ?? [])
        {
            response.Messages.Add(new ApplicationMessage
            {
                MessageId = message.Id,
                ApplicationId = message.ApplicationId,
                MessageText = message.Text,
                MessageType = message.Type.ToString(),
                MessageHeader = message.MessageHeader,
                SharedWithDfe = message.SharedWithDfe,
                SharedWithOfqual = message.SharedWithOfqual,
                SharedWithSkillsEngland = message.SharedWithSkillsEngland,
                SharedWithAwardingOrganisation = message.SharedWithAwardingOrganisation,
                SentByName = message.SentByName,
                SentByEmail = message.SentByEmail,
                SentAt = message.SentAt
            });
        }

        return response;
    }
}