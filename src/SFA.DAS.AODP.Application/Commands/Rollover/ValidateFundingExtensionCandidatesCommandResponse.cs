using SFA.DAS.AODP.Application.Services.Validation;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Application.Commands.Rollover
{
    [ExcludeFromCodeCoverage]
    public class ValidateFundingExtensionCandidatesCommandResponse
    {
        public bool IsValid { get; set; }

        public int TotalCandidates { get; set; }

        public int FailedCandidateCount { get; set; }

        public List<ValidationFailureGroup> FailureSummary { get; set; }
        public byte[]? ValidatedCandidateFile { get; set; }
    }

}