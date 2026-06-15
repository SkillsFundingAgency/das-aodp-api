using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Rollover
{
    [ExcludeFromCodeCoverage]
    public class ValidateFundingExtensionCandidatesCommandResponse
    {
        public bool IsValid { get; set; }

        public ValidationFailureSummary? ValidationFailureSummary { get; set; }

        public FundingExtensionSummary? ValidationSuccessSummary { get; set; }
        
    }

    [ExcludeFromCodeCoverage]
    public class ValidationFailureSummary
    {
        public int FailedCandidateCount { get; set; }
        public byte[]? ValidatedCandidateFile { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class FundingExtensionSummary
    {
        public int TotalCandidatesCount { get; set; }
        public int TotalReviewedCandidatesCount { get; set; }
        public int PendingExtendedCandidatesCount { get; set; }
        public int PendingExcludedCandidatesCount { get; set; }
        public int PendingReviewCandidatesCount { get; set; }

    }

}