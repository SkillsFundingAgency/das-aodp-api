namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class Page
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid Key { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<Question> Questions { get; set; } = new();
    public virtual Section Section { get; set; }
}
