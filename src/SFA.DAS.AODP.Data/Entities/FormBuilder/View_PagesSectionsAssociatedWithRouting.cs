namespace SFA.DAS.AODP.Data.Entities.FormBuilder
{
    public class View_PagesSectionsAssociatedWithRouting
    {
        public Guid SourceQuestionId { get; set; }
        public Guid SourcePageId { get; set; }
        public Guid SourceSectionId { get; set; }
        public Guid? NextPageId { get; set; }
        public Guid? NextPageSectionId { get; set; }
        public Guid? NextSectionId { get; set; }
    }
}