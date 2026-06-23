using SFA.DAS.AODP.Application.Commands.Rollover;

namespace SFA.DAS.AODP.Application.Services.Validation
{
    public class FundingExtensionValidationResult
    {
        public bool IsValid { get; set; }

        public int FailedCandidateCount { get; set; }

        public List<CandidateValidationResult> Candidates { get; set; }
            = new List<CandidateValidationResult>();

        public List<ValidationFailureGroup> FailureSummary { get; set; }
            = new List<ValidationFailureGroup>();
    }

    public class CandidateValidationResult
    {
        public required RolloverCandidateForValidation CandidateDetails { get; set; }

        public List<ValidationFailure> Errors { get; set; }
            = new List<ValidationFailure>();

        public bool IsValid => Errors.Count == 0;
    }

    public class ValidationFailure
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ValidationFailureGroup
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Count { get; set; }
    }

}
