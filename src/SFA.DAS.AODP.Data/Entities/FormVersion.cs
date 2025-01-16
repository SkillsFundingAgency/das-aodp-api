namespace SFA.DAS.AODP.Data.Entities;

public class FormVersion
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public bool Published { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public Form Form { get; set; }
    public DateTime DateCreated { get; set; }
}