namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class Section
{
    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public Guid Key { get; set; }
    public int Order { get; set; }
    public string Title { get; set; }
    public virtual List<Page> Pages { get; set; }
    public virtual FormVersion FormVersion { get; set; }
    public virtual View_SectionPageCount View_SectionPageCount { get; set; }
}
