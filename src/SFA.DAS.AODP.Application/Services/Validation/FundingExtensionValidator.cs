using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Constants;
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
            FundingExtensionCandidateValidationContext fundingExtensionCandidateValidationContext,
            CancellationToken cancellationToken)
        {
            var response = new FundingExtensionValidationResult();

            foreach (var row in fundingExtensionCandidates)
            {
                var result = new CandidateValidationResult
                {
                    CandidateDetails = row
                };

                foreach (var rule in _rules)
                {
                    rule(result, fundingExtensionCandidateValidationContext);
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

        // -------------------------
        // Validation Rules
        // -------------------------

        private void ValidateRolloverCandidateExists(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            var key = new CandidateKey(result.CandidateDetails.Qan, result.CandidateDetails.FundingStreamName);

            if (!ctx.CandidatesInDb.Contains(key))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "QAN",
                    Message = "This candidate is no longer viable for RollOver"
                });
            }
        }

        private void ValidateRolloverWorkflowCandidateExists(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            var key = new CandidateKey(result.CandidateDetails.Qan, result.CandidateDetails.FundingStreamName);

            if (!ctx.WorkflowCandidatesInDb.Contains(key))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "QAN",
                    Message = "This candidate was not in the original scope for Rollover"
                });
            }
        }

        private void ValidateStatus(
         CandidateValidationResult result,
         FundingExtensionCandidateValidationContext ctx)
        {
            if (!Enum.TryParse<RolloverStatus>(result.CandidateDetails.RollOverStatus, true, out var parsed))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "RolloverStatus",
                    Message = $"This candidate has an invalid RollOver Status ({string.Join(", ", AllowedStatusForUserInput)})"
                });
                return;
            }

            if (!AllowedStatusForUserInput.Contains(parsed))
            {
                result.Errors.Add(new ValidationFailure
                {
                    Field = "RolloverStatus",
                    Message = $"This candidate has an invalid RollOver Status ({string.Join(", ", AllowedStatusForUserInput)})"
                });
            }

        }


        private void ValidateExclusionReason(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            var raw = result.CandidateDetails.RollOverStatus;

            // Convert string → enum
            if (!Enum.TryParse<RolloverStatus>(raw, true, out var status))
            {
                //result.Errors.Add(new ValidationFailure
                //{
                //    Field = "RolloverStatus",
                //    Message = $"Invalid RollOver Status '{raw}'"
                //});
                return;
            }

            // Now compare enum → enum
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
