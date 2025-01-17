namespace SFA.DAS.AODP.Models.Forms.FormBuilder;

public class Form
{
    public Guid Id { get; set; }
    public bool Archived { get; set; } = false;
    public List<FormVersion> FormVersion { get; set; }

    public Form()
    {
        FormVersion = new List<FormVersion>();
    }
}