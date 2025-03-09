using SFA.DAS.AODP.Data.Entities.Offer;

namespace SFA.DAS.AODP.Data.Entities.Application
{
    public class ApplicationReviewFunding
    {
        public Guid Id { get; set; }
        public Guid ApplicationReviewId { get; set; }
        public Guid FundingOfferId { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Comments { get; set; }

        public virtual FundingOffer FundingOffer { get; set; }
    }
}
