using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Constants;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Services
{
    public interface IRolloverFundingExtensionValidator
    {
        ValidateFundingExtensionCandidatesCommandResponse Validate(
            List<FundingExtensionCandidate> fundingExtensionCandidates,
            FundingExtensionCandidateValidationContext fundingExtensionCandidateValidationContext,
            CancellationToken cancellationToken);
    }

    public class RolloverFundingExtensionValidator : IRolloverFundingExtensionValidator
    {
        private readonly List<Action<CandidateValidationResult, FundingExtensionCandidateValidationContext>> _rules;

        public RolloverFundingExtensionValidator()
        {
            _rules = new()
            {
                ValidateRolloverCandidateExists,
                ValidateRolloverWorkflowCandidateExists,
                ValidateStatus,
                ValidateEndDateInFuture,
                ValidateExclusionReason
            };
        }

        public ValidateFundingExtensionCandidatesCommandResponse Validate(
            List<FundingExtensionCandidate> fundingExtensionCandidates,
            FundingExtensionCandidateValidationContext fundingExtensionCandidateValidationContext,
            CancellationToken cancellationToken)
        {
            var response = new ValidateFundingExtensionCandidatesCommandResponse
            {
                TotalCandidates = fundingExtensionCandidates.Count
            };

            foreach (var row in fundingExtensionCandidates)
            {
                var result = new CandidateValidationResult
                {
                    Qan = row.Qan,
                    FundingStreamName = row.FundingStreamName,
                    RowNumber = row.RowNumber,
                    RolloverStatus = row.RolloverStatus,
                    ExclusionReason = row.ExclusionReason,
                    ProposedFundingEndDate = row.ProposedFundingEndDate
                };

                foreach (var rule in _rules)
                {
                    rule(result, fundingExtensionCandidateValidationContext);
                }

                result.IsValid = result.Errors.Count == 0;
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
            var key = new CandidateKey(result.Qan, result.FundingStreamName);

            if (!ctx.CandidatesInDb.Contains(key))
            {
                result.Errors.Add(new ValidationError
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
            var key = new CandidateKey(result.Qan, result.FundingStreamName);

            if (!ctx.WorkflowCandidatesInDb.Contains(key))
            {
                result.Errors.Add(new ValidationError
                {
                    Field = "QAN",
                    Message = "Candidate was not in the original scope for Rollover"
                });
            }
        }

        private void ValidateStatus(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            if (!RolloverStatuses.All.Contains(result.RolloverStatus))
            {
                result.Errors.Add(new ValidationError
                {
                    Field = "RolloverStatus",
                    Message = $"This candidate has an invalid RollOver Status ({RolloverStatuses.ToList()})"
                });
            }
        }

        private void ValidateEndDateInFuture(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            if (result.ProposedFundingEndDate <= DateTime.UtcNow.Date)
            {
                result.Errors.Add(new ValidationError
                {
                    Field = "ProposedFundingEndDate",
                    Message = "This candidate has an invalid Proposed Funding End Date (End Date must be in the future)"
                });
            }
        }

        private void ValidateExclusionReason(
            CandidateValidationResult result,
            FundingExtensionCandidateValidationContext ctx)
        {
            if (result.RolloverStatus == "To Exclude" &&
                string.IsNullOrWhiteSpace(result.ExclusionReason))
            {
                result.Errors.Add(new ValidationError
                {
                    Field = "ExclusionReason",
                    Message = "The candidate is missing an Exclusion Reason"
                });
            }
        }
    }

}
