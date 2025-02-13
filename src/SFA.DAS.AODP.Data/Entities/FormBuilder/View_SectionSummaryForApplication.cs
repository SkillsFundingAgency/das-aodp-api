namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class View_SectionSummaryForApplication
{
    public Guid SectionId { get; set; }
    public Guid ApplicationId { get; set; }
    public int? RemainingPages { get; set; }
    public int? SkippedPages { get; set; }
    public int? TotalPages { get; set; }

}