namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class View_QuestionRoutingDetail
{
    public Guid FormVersionId { get; set; }
    public Guid SectionId { get; set; }
    public Guid PageId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid OptionId { get; set; }

    public Guid? NextPageId { get; set; }
    public Guid? NextSectionId { get; set; }

    public string QuestionTitle { get; set; }
    public string PageTitle { get; set; }
    public string SectionTitle { get; set; }
    public string OptionValue { get; set; }
    public string? NextPageTitle { get; set; }
    public string? NextSectionTitle { get; set; }

    public int QuestionOrder { get; set; }
    public int PageOrder { get; set; }
    public int SectionOrder { get; set; }
    public int OptionOrder { get; set; }
    public int? NextPageOrder { get; set; }
    public int? NextSectionOrder { get; set; }

    public bool EndForm { get; set; }
    public bool EndSection { get; set; }
}