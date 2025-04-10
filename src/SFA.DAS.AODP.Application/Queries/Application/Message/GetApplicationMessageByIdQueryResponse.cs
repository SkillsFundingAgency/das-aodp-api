﻿namespace SFA.DAS.AODP.Application.Queries.Application.Message;

public class GetApplicationMessageByIdQueryResponse
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

    public static implicit operator GetApplicationMessageByIdQueryResponse(Data.Entities.Application.Message message)
    {
        return new()
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

        };
    }
}