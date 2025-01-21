﻿namespace SFA.DAS.AODP.Data.Entities;

public class Section
{
    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid Key { get; set; }
    public int Order { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<Page> Pages { get; set; }

    public Section()
    {
        Pages = new List<Page>();
    }
}
