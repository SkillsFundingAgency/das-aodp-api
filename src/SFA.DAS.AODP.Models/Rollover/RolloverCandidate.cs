namespace SFA.DAS.AODP.Models.Rollover
{
    public class RolloverCandidate
    {
        public string Qan { get; init; }
        public string Title { get; init; }
        public string AwardingOrganisation { get; init; }
        public string FundingOffer { get; init; }
        public DateTime? FundingApprovalEndDate { get; init; }
        public bool IsActive { get; set; }
    }
}