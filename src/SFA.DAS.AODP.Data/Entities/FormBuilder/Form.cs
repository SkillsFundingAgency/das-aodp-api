namespace SFA.DAS.AODP.Data.Entities;

public class Form
{
    public Guid Id { get; set; }
    public bool Archived { get; set; } = false;
    public List<FormVersion> Versions { get; set; } = new List<FormVersion>();
}
