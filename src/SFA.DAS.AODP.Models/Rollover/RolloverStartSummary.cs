using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Rollover
{
    [ExcludeFromCodeCoverage]
    public class RolloverStartSummary
    {
        public int TotalCandidatesCount { get; set; }
        public int CandidatesEligibleCount { get; set; }
        public int CandidatesIneligibleCount { get; set; }
        public int CandidatesRemainingCount { get; set; }
    }
}
