namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class Route
{
    public Guid Id { get; set; }
    public Guid SourceQuestionId { get; set; }
    public Guid? NextPageId { get; set; }
    public Guid? NextSectionId { get; set; }
    public Guid SourceOptionId { get; set; }
    public bool EndSection { get; set; }
    public bool EndForm { get; set; }

    public virtual Question SourceQuestion { get; set; }
    public virtual Page NextPage { get; set; }
    public virtual Section NextSection { get; set; }
}