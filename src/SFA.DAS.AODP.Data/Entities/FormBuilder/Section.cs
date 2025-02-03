namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class Section
{
    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid Key { get; set; }
    public int Order { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int? PagesCount { get; set; }
    public virtual List<Page> Pages { get; set; }
    public virtual FormVersion FormVersion { get; set; }
}
