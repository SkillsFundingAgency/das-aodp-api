﻿namespace SFA.DAS.AODP.Models.Archived.Forms.Application;

public class Application
{
    public int Id { get; set; }
    public int FormSchemaId { get; set; }
    public List<AnsweredQuestion> AnsweredQuestions { get; set; } = new List<AnsweredQuestion>();
}
