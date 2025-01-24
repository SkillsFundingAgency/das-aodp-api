using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities;

public class FormVersion
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Version { get; set; }
    [Column(TypeName = "nvarchar(100)")]
    public FormStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime DateCreated { get; set; }
    public virtual Form Form { get; set; } = new Form();
    public virtual List<Section> Sections { get; set; } = new List<Section>();
}

public enum FormStatus
{
    Draft,
    Published,
    Archived
}
