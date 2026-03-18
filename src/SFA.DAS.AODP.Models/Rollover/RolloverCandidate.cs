using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Rollover
{
    [ExcludeFromCodeCoverage]
    public class RolloverCandidate
    {
        public Guid Id { get; set; }
        public Guid QualificationVersionId { get; set; }
        public string? QualificationNumber { get; init; }
        public Guid FundingOfferId { get; set; }
        public string? FundingOfferName { get; init; }
        public string? AcademicYear { get; set; }
    }
}