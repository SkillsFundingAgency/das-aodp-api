﻿using SFA.DAS.AODP.Models.Form;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities;

public class FormVersion
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Version { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public DateTime DateCreated { get; set; }
    public virtual Form Form { get; set; } = new Form();
    public virtual List<Section> Sections { get; set; } = new List<Section>();
}


