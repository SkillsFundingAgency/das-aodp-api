namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class View_AvailableQuestionsForRouting
{
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
    public Guid PageId { get; set; }
    public Guid QuestionId { get; set; }

    public string QuestionTitle { get; set; }
    public string PageTitle { get; set; }
    public string SectionTitle { get; set; }

    public int QuestionOrder { get; set; }
    public int PageOrder { get; set; }
    public int SectionOrder { get; set; }
}
