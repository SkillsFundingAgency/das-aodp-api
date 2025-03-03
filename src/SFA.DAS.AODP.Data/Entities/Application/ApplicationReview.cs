namespace SFA.DAS.AODP.Data.Entities.Application
{
    public class ApplicationReview
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }

        public bool SharedWithSkillsEngland { get; set; }

        public bool SharedWithOfqual { get; set; }

        public virtual Application Application { get; set; }
        public virtual List<ApplicationReviewFeedback> ApplicationReviewFeedbacks { get; set; }
        public virtual ApplicationReviewDecision ApplicationReviewDecision { get; set; }

    }
}
