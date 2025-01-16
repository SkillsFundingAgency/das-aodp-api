namespace SFA.DAS.AODP.Data.Entities;

public class Form
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public List<FormVersion> Versions { get; set; }
}
