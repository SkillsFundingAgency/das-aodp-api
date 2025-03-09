namespace SFA.DAS.AODP.Data.Entities.Application
{
    public class ApplicationReviewFeedback
    {
        public Guid Id { get; set; }
        public Guid ApplicationReviewId { get; set; }
        public string? Owner { get; set; }
        public string Status { get; set; }
        public string? Comments { get; set; }
        public bool NewMessage { get; set; }
        public string Type { get; set; }

        public virtual ApplicationReview ApplicationReview { get; set; }

    }
}
