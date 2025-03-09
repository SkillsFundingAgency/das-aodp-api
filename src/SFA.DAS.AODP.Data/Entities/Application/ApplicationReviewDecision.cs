namespace SFA.DAS.AODP.Data.Entities.Application
{
    public class ApplicationReviewDecision
    {
        public Guid Id { get; set; }
        public Guid ApplicationReviewId { get; set; }
        public string? Status { get; set; }
        public string? Comments { get; set; }
    }
}
