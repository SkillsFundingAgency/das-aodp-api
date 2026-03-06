namespace SFA.DAS.AODP.Models.Rollover
{
    public class RolloverCandidate
    {
        public Guid Id { get; set; }

        public Guid QualificationVersionId { get; set; }

        public Guid FundingOfferId { get; set; }

        public bool IsActive { get; set; }

        public string Qan { get; init; }

        public string Title { get; init; } 

        public string AwardingOrganisation { get; init; }

        public string FundingOffer { get; init; }

        public DateTime? FundingApprovalEndDate { get; init; }
    }
}