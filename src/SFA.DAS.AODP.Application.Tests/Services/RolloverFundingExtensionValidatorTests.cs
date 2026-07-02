using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Services
{
    public class RolloverFundingExtensionValidatorTests
    {
        private readonly RolloverFundingExtensionValidator _sut;

        public RolloverFundingExtensionValidatorTests()
        {
            _sut = new RolloverFundingExtensionValidator();
        }

        private FundingExtensionCandidateValidationContext ValidationContext(
            IEnumerable<CandidateKey>? candidates = null,
            IEnumerable<CandidateKey>? workflow = null)
        {
            return new FundingExtensionCandidateValidationContext(
                candidates?.ToHashSet() ?? new HashSet<CandidateKey>(),
                workflow?.ToHashSet() ?? new HashSet<CandidateKey>(),
                workflow?.ToHashSet() ?? new HashSet<CandidateKey>());
        }

        private FundingExtensionCandidate CandidateRow(
            int row = 1,
            string qan = "12345",
            string stream = "LegalEntitlementEnglishandMaths",
            string status = "To Extend",
            string? reason = "Some reason",
            DateTime? endDate = null)
        {
            return new FundingExtensionCandidate
            {
                RowNumber = row,
                Qan = qan,
                FundingStreamName = stream,
                RolloverStatus = status,
                ExclusionReason = reason,
                ProposedFundingEndDate = endDate ?? DateTime.UtcNow.AddDays(10)
            };
        }

        [Fact]
        public void Validate_ShouldFail_WhenCandidateNotInDb()
        {
            var candidate = CandidateRow();
            var ctx = ValidationContext(candidates: Array.Empty<CandidateKey>());

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);
            Assert.Contains(result.Candidates[0].Errors,
                e => e.Message.Contains("no longer viable"));
        }

        [Fact]
        public void Validate_ShouldFail_WhenCandidateNotInWorkflow()
        {
            var candidate = CandidateRow();
            var ctx = ValidationContext(
                candidates: [new CandidateKey("12345", "LegalEntitlementEnglishandMaths")],
                workflow: Array.Empty<CandidateKey>());

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);
            Assert.Contains(result.Candidates[0].Errors,
                e => e.Message.Contains("not in the original scope"));
        }


        [Fact]
        public void Validate_ShouldFail_WhenStatusInvalid()
        {
            var candidate = CandidateRow(status: "BadStatus");
            var ctx = ValidationContext(
                candidates: [new CandidateKey("12345", "LegalEntitlementEnglishandMaths")],
                workflow: [new CandidateKey("12345", "LegalEntitlementEnglishandMaths")]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);
            Assert.Contains(result.Candidates[0].Errors,
                e => e.Field == "RolloverStatus");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void Validate_ShouldNotFail_DueToEndDate(int dayOffset)
        {
            var candidate = CandidateRow(endDate: DateTime.UtcNow.Date.AddDays(dayOffset));
            var ctx = ValidationContext(
                candidates: [new CandidateKey("12345", "LegalEntitlementEnglishandMaths")],
                workflow: [new CandidateKey("12345", "LegalEntitlementEnglishandMaths")]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.True(result.IsValid);
            Assert.Empty(result.Candidates[0].Errors);
        }


        [Fact]
        public void Validate_ShouldFail_WhenToExcludeAndNoReason()
        {
            var candidate = CandidateRow(status: "To Exclude", reason: "");
            var ctx = ValidationContext(
                candidates: [new CandidateKey("12345", "LegalEntitlementEnglishandMaths")],
                workflow: [new CandidateKey("12345", "LegalEntitlementEnglishandMaths")]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);
            Assert.Contains(result.Candidates[0].Errors,
                e => e.Field == "ExclusionReason");
        }

        [Fact]
        public void Validate_ShouldCaptureMultipleErrors()
        {
            var candidate = CandidateRow(
                qan: "99999",
                status: "BadStatus",
                reason: "",
                endDate: DateTime.UtcNow.AddDays(-10));

            var ctx = ValidationContext(
                candidates: Array.Empty<CandidateKey>(),
                workflow: Array.Empty<CandidateKey>());

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);
            Assert.True(result.Candidates[0].Errors.Count >= 3);
        }


        [Fact]
        public void Validate_ShouldPass_WhenCandidateIsValid()
        {
            var candidate = CandidateRow();
            var key = new CandidateKey("12345", "LegalEntitlementEnglishandMaths");

            var ctx = ValidationContext(
                candidates: [key],
                workflow: [key]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.True(result.IsValid);
            Assert.Empty(result.Candidates[0].Errors);
        }

        [Fact]
        public void Validate_ShouldFail_WhenStatusWhitespace()
        {
            var candidate = CandidateRow(status: "   ");
            var key = new CandidateKey("12345", "LegalEntitlementEnglishandMaths");

            var ctx = ValidationContext(
                candidates: [key],
                workflow: [key]);

            var result = _sut.Validate([candidate], ctx, CancellationToken.None);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_ShouldHandleDuplicateRows()
        {
            var candidate1 = CandidateRow(row: 1);
            var candidate2 = CandidateRow(row: 2);

            var key = new CandidateKey("12345", "LegalEntitlementEnglishandMaths");

            var ctx = ValidationContext(
                candidates: [key],
                workflow: [key]);

            var result = _sut.Validate([candidate1, candidate2], ctx, CancellationToken.None);

            Assert.Equal(2, result.TotalCandidates);
        }
    }
}
