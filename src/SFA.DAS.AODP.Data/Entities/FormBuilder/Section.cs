namespace SFA.DAS.AODP.Data.Entities;

public class Section
{
    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid Key { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<Page> Pages { get; set; } = new List<Page>();
    public virtual FormVersion FormVersion { get; set; } = new();
}
