using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.Validation;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Services
{
    public class RolloverFundingExtensionValidatorTests
    {
        private readonly FundingExtensionValidator _sut;

        public RolloverFundingExtensionValidatorTests()
        {
            _sut = new FundingExtensionValidator();
        }

        private FundingExtensionCandidateValidationContext ValidationContext(
            IEnumerable<CandidateKey>? incomingCandidates = null,
            IEnumerable<CandidateKey>? candidatesInDb = null,
            IEnumerable<CandidateKey>? workflowCandidatesInDb = null)
        {
            return new FundingExtensionCandidateValidationContext(
                incomingCandidates?.ToHashSet() ?? new HashSet<CandidateKey>(),
                candidatesInDb?.ToHashSet() ?? new HashSet<CandidateKey>(),
                workflowCandidatesInDb?.ToHashSet() ?? new HashSet<CandidateKey>());
        }

        private RolloverCandidateForValidation CandidateRow(
            int row = 1,
            string qan = "12345",
            string stream = "LegalEntitlementEnglishandMaths",
            string status = "To Extend",
            string? reason = "Some reason",
            DateTime? endDate = null)
        {
            return new RolloverCandidateForValidation
            {
                RowNumber = row,
                Qan = qan,
                FundingStreamName = stream,
                RollOverStatus = status,
                ExclusionReason = reason,
                ProposedFundingApprovalEndDate = endDate ?? DateTime.UtcNow.AddDays(10)
            };
        }

        [Fact]
        public void Validate_ShouldFail_WhenCandidateNotInDb()
        {
            var candidate = CandidateRow();

            var ctx = ValidationContext(
                candidatesInDb: Array.Empty<CandidateKey>(),
                workflowCandidatesInDb:
                [
                    new CandidateKey("12345", "LegalEntitlementEnglishandMaths")
                ]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);

            Assert.Contains(
                result.Candidates[0].Errors,
                e => e.Message.Contains("no longer viable"));
        }

        [Fact]
        public void Validate_ShouldFail_WhenCandidateNotInWorkflow()
        {
            var candidate = CandidateRow();

            var ctx = ValidationContext(
                candidatesInDb:
                [
                    new CandidateKey("12345", "LegalEntitlementEnglishandMaths")
                ],
                workflowCandidatesInDb: Array.Empty<CandidateKey>());

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);

            Assert.Contains(
                result.Candidates[0].Errors,
                e => e.Message.Contains("not in the original scope"));
        }


        [Fact]
        public void Validate_ShouldFail_WhenStatusInvalid()
        {
            var candidate = CandidateRow(status: "BadStatus");

            var key = new CandidateKey("12345", "LegalEntitlementEnglishandMaths");

            var ctx = ValidationContext(
                candidatesInDb: [key],
                workflowCandidatesInDb: [key]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);

            Assert.Contains(
                result.Candidates[0].Errors,
                e => e.Field == "RolloverStatus" &&
                     e.Message.StartsWith("This candidate has an invalid RollOver Status"));
        }


        [Fact]
        public void Validate_ShouldFail_WhenToExcludeAndNoReason()
        {
            var candidate = CandidateRow(
                status: "To Exclude",
                reason: "");

            var key = new CandidateKey("12345", "LegalEntitlementEnglishandMaths");

            var ctx = ValidationContext(
                candidatesInDb: [key],
                workflowCandidatesInDb: [key]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);

            Assert.Contains(
                result.Candidates[0].Errors,
                e => e.Field == "ExclusionReason" &&
                     e.Message.StartsWith("The candidate is missing an Exclusion Reason"));
        }


        [Fact]
        public void Validate_ShouldCaptureMultipleErrors()
        {
            var candidate = CandidateRow(
                qan: "99999",
                status: "BadStatus",
                reason: "");

            var ctx = ValidationContext(
                candidatesInDb: Array.Empty<CandidateKey>(),
                workflowCandidatesInDb: Array.Empty<CandidateKey>());

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);

            Assert.True(result.Candidates[0].Errors.Count >= 3);
        }


        [Fact]
        public void Validate_ShouldPass_WhenCandidateIsValid()
        {
            var candidate = CandidateRow();

            var key = new CandidateKey(
                "12345",
                "LegalEntitlementEnglishandMaths");

            var ctx = ValidationContext(
                candidatesInDb: [key],
                workflowCandidatesInDb: [key]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.True(result.IsValid);
            Assert.Empty(result.Candidates[0].Errors);
        }

        [Fact]
        public void Validate_ShouldFail_WhenStatusWhitespace()
        {
            var candidate = CandidateRow(status: "   ");

            var key = new CandidateKey(
                "12345",
                "LegalEntitlementEnglishandMaths");

            var ctx = ValidationContext(
                candidatesInDb: [key],
                workflowCandidatesInDb: [key]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_ShouldHandleDuplicateRows()
        {
            var candidate1 = CandidateRow(row: 1);
            var candidate2 = CandidateRow(row: 2);

            var key = new CandidateKey(
                "12345",
                "LegalEntitlementEnglishandMaths");

            var ctx = ValidationContext(
                candidatesInDb: [key],
                workflowCandidatesInDb: [key]);

            var result = _sut.Validate(
                [candidate1, candidate2],
                ctx,
                CancellationToken.None);

            Assert.Equal(2, result.Candidates.Count);
        }

        [Fact]
        public void Validate_ShouldBuildFailureSummary()
        {
            var candidate1 = CandidateRow(qan: "111", status: "BadStatus");
            var candidate2 = CandidateRow(qan: "222", status: "BadStatus");

            var ctx = ValidationContext(
                candidatesInDb: [new CandidateKey("111", "LegalEntitlementEnglishandMaths"),
                         new CandidateKey("222", "LegalEntitlementEnglishandMaths")],
                workflowCandidatesInDb: [new CandidateKey("111", "LegalEntitlementEnglishandMaths"),
                                 new CandidateKey("222", "LegalEntitlementEnglishandMaths")]);

            var result = _sut.Validate([candidate1, candidate2], ctx, CancellationToken.None);

            var summary = result.FailureSummary.Single(x => x.Field == "RolloverStatus");

            Assert.Equal(2, summary.Count);
        }

        [Fact]
        public void Validate_ShouldCalculateFailedCandidateCount()
        {
            var valid = CandidateRow(qan: "111");
            var invalid = CandidateRow(qan: "999", status: "BadStatus");

            var key = new CandidateKey("111", "LegalEntitlementEnglishandMaths");

            var ctx = ValidationContext(
                candidatesInDb: [key],
                workflowCandidatesInDb: [key]);

            var result = _sut.Validate([valid, invalid], ctx, CancellationToken.None);

            Assert.Equal(2, result.Candidates.Count);
            Assert.Equal(1, result.FailedCandidateCount);
            Assert.False(result.IsValid);
        }
    }
}
