using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Rollover
{
    [ExcludeFromCodeCoverage]
    public class RolloverCandidate
    {
        public Guid Id { get; set; }
        public Guid QualificationVersionId { get; set; }
        public string? QualificationNumber { get; set; }
        public Guid FundingOfferId { get; set; }
        public string? FundingOfferName { get; set; }
        public string? AcademicYear { get; set; }
        public int RolloverRound { get; set; }
        public DateTime? PreviousFundingEndDate { get;  set; }
        public DateTime? NewFundingEndDate { get;  set; }
    }
}