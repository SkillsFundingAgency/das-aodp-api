namespace SFA.DAS.AODP.Data.Entities.FormBuilder;

public class View_RemainingPagesBySectionForApplication
{
    public Guid ApplicationId { get; set; }
    public Guid SectionId { get; set; }
    public int PageCount { get; set; }

    public Application.Application Application { get; set; }
}