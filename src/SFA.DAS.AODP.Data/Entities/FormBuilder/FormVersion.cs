using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AODP.Data.Entities;

public class FormVersion
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string Name { get; set; }
    public DateTime Version { get; set; }
    [Column(TypeName = "nvarchar(100)")]
    public FormStatus Status { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public DateTime DateCreated { get; set; }
    public Form Form { get; set; }
}

public enum FormStatus
{
    Draft,
    Published,
    Archived
}
