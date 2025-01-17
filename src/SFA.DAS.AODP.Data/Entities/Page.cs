﻿namespace SFA.DAS.AODP.Data.Entities;

public class Page
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public int? NextPageId { get; set; }
    public bool Archived { get; set; }
    public List<Question> Questions { get; set; }

    public Page()
    {
        Questions = new List<Question>();
    }
}
