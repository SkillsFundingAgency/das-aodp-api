namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class Route
{
    public Guid Id { get; set; }
    public Guid SourceQuestionId { get; set; }
    public Guid NextQuestionId { get; set; }
    public Guid NextSectionId { get; set; }
    public Guid OptionId { get; set; }
    public bool EndSection { get; set; }
    public bool EndForm { get; set; }

    public Question SourceQuestion { get; set; }
    public Page NextPage { get; set; }
    public Section NextSection { get; set; }
}
