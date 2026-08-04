using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Services.Validation
{
    public interface IRolloverFundingExtensionValidator
    {
        FundingExtensionValidationResult Validate(
            List<RolloverCandidateForValidation> fundingExtensionCandidates,
            FundingExtensionCandidateValidationContext fundingExtensionCandidateValidationContext,
            CancellationToken cancellationToken);
    }

    public class FundingExtensionValidator : IRolloverFundingExtensionValidator
    {
        private readonly List<Action<CandidateValidationResult, FundingExtensionCandidateValidationContext>> _rules;

        public static readonly RolloverStatus[] AllowedStatusForUserInput =
        [
            RolloverStatus.Extended,
            RolloverStatus.Excluded
        ];

        public FundingExtensionValidator()
        {
            _rules = new()
            {
                ValidateRolloverCandidateExists,
                ValidateRolloverWorkflowCandidateExists,
                ValidateStatus,
                ValidateExclusionReason
            };
        }

        public FundingExtensionValidationResult Validate(
            List<RolloverCandidateForValidation> fundingExtensionCandidates,
            FundingExtensionCandidateValidationContext ctx,
            CancellationToken cancellationToken)
        {
            var response = new FundingExtensionValidationResult();

            foreach (var row in fundingExtensionCandidates)
            {
                var result = new CandidateValidationResult
                {
                    CandidateDetails = row
                };

                ValidateRequiredFields(result, ctx);

                if (result.IsValid)
                {
                    foreach (var rule in _rules)
                    {
                        rule(result, ctx);
                    }
                }

                response.Candidates.Add(result);
            }

            response.FailedCandidateCount = response.Candidates.Count(c => !c.IsValid);
            response.IsValid = response.FailedCandidateCount == 0;

            response.FailureSummary = response.Candidates
                .SelectMany(c => c.Errors)
                .GroupBy(e => new { e.Field, e.Message })
                .Select(g => new ValidationFailureGroup
                {
                    Field = g.Key.Field,
                    Message = g.Key.Message,
                    Count = g.Count()
                })
                .ToList();

            return response;
        }

        // -------------------------------------------------------
        // ⭐ RULE 1 — Required Fields (QAN, FundingStreamName, Status)
        // -------------------------------------------------------
        private void ValidateRequiredFields(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            if (string.IsNullOrWhiteSpace(result.CandidateDetails?.Qan))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "QAN",
                    Message = "QAN is required"
                });
            }

            if (string.IsNullOrWhiteSpace(result.CandidateDetails?.FundingStreamName))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "FundingStreamName",
                    Message = "Funding Stream Name is required"
                });
            }

            if (string.IsNullOrWhiteSpace(result.CandidateDetails?.RollOverStatus))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "RolloverStatus",
                    Message = "Rollover Status is required"
                });
            }
        }

        // -------------------------------------------------------
        // RULE 2 — Candidate must exist in DB
        // -------------------------------------------------------
        private void ValidateRolloverCandidateExists(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            var key = new CandidateKey(
                result.CandidateDetails.Qan!,
                result.CandidateDetails.FundingStreamName);

            if (!ctx.CandidatesInDb.Contains(key))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "QAN",
                    Message = "This candidate is no longer viable for RollOver"
                });
            }
        }

        // -------------------------------------------------------
        // RULE 3 — Candidate must exist in workflow scope
        // -------------------------------------------------------
        private void ValidateRolloverWorkflowCandidateExists(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            var key = new CandidateKey(
                result.CandidateDetails.Qan!,
                result.CandidateDetails.FundingStreamName);

            if (!ctx.WorkflowCandidatesInDb.Contains(key))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "QAN",
                    Message = "This candidate was not in the original scope for Rollover"
                });
            }
        }

        // -------------------------------------------------------
        // RULE 4 — Status must be valid and allowed
        // -------------------------------------------------------
        private void ValidateStatus(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            if (!Enum.TryParse<RolloverStatus>(result.CandidateDetails.RollOverStatus, true, out var parsed) ||
                !AllowedStatusForUserInput.Contains(parsed))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "RolloverStatus",
                    Message = $"This candidate has an invalid RollOver Status ({string.Join(", ", AllowedStatusForUserInput)})"
                });
            }
        }

        // -------------------------------------------------------
        // RULE 5 — ExclusionReason required when status = Excluded
        // -------------------------------------------------------
        private void ValidateExclusionReason(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            if (!Enum.TryParse<RolloverStatus>(result.CandidateDetails.RollOverStatus, true, out var status))
                return;

            if (status == RolloverStatus.Excluded &&
                string.IsNullOrWhiteSpace(result.CandidateDetails.ExclusionReason))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "ExclusionReason",
                    Message = "The candidate is missing an Exclusion Reason"
                });
            }
        }
    }
}
