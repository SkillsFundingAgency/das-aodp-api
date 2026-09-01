using AutoFixture;
using AutoFixture.AutoMoq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Models.Rollover;
using Xunit;

namespace SFA.DAS.AODP.Application.Tests.Services.Rollover
{
    public class FundingExtensionProjectionServiceTests
    {
        private readonly FundingExtensionProjectionService _service;
        private readonly IFixture _fixture;
        private readonly DateTime _defaultEndDate = DateTime.UtcNow.AddDays(20);

        public FundingExtensionProjectionServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _service = new FundingExtensionProjectionService();
        }


        [Fact]
        public void ProjectSummary_UsesUploadedStatus_WhenUploadedOverridesDb()
        {
            // Arrange
            var dbCandidates = new List<RolloverCandidateStatusItem>
            {
                new() { Qan = "123", FundingStreamName = "FS1", RolloverStatus = RolloverStatus.NeedsReview },
                new() { Qan = "456", FundingStreamName = "FS1", RolloverStatus = RolloverStatus.NeedsReview }
            };

            var uploaded = new List<RolloverCandidateForValidation>
            {
                new() { Qan = "123", FundingStreamName = "FS1", RollOverStatus = "Extended", ProposedFundingApprovalEndDate =  _defaultEndDate }
            };

            // Act
            var result = _service.ProjectSummary(dbCandidates, uploaded);

            // Assert
            Assert.Equal(2, result.TotalCandidatesCount);
            Assert.Equal(1, result.CandidatesExtendedInUploadCount);
            Assert.Equal(1, result.TotalCandidatesToBeExtendedCount);
            Assert.Equal(0, result.TotalCandidatesToBeExcludedCount);
            Assert.Equal(1, result.TotalCandidatesToBeReviewedCount);
        }

        [Fact]
        public void ProjectSummary_UsesDistinctUploadedCandidates()
        {
            // Arrange
            var dbCandidates = new List<RolloverCandidateStatusItem>
            {
                new() { Qan = "123", FundingStreamName = "FS1", RolloverStatus = RolloverStatus.NeedsReview }
            };

            var uploaded = new List<RolloverCandidateForValidation>
            {
                new() { Qan = "123", FundingStreamName = "FS1", RollOverStatus = "Extended", ProposedFundingApprovalEndDate =  _defaultEndDate },
                new() { Qan = "123", FundingStreamName = "FS1", RollOverStatus = "Excluded", ProposedFundingApprovalEndDate =  _defaultEndDate } // duplicate
            };

            // Act
            var result = _service.ProjectSummary(dbCandidates, uploaded);

            // Assert
            Assert.Equal(1, result.CandidatesExtendedInUploadCount); // raw count
            Assert.Equal(1, result.TotalCandidatesToBeExtendedCount); // distinct override
        }


        [Fact]
        public void ProjectSummary_UsesDbStatuses_WhenNoUploadedCandidates()
        {
            // Arrange
            var dbCandidates = new List<RolloverCandidateStatusItem>
            {
                new() { Qan = "123", FundingStreamName = "FS1", RolloverStatus = RolloverStatus.Extended },
                new() { Qan = "456", FundingStreamName = "FS1", RolloverStatus = RolloverStatus.Excluded },
                new() { Qan = "789", FundingStreamName = "FS1", RolloverStatus = RolloverStatus.NeedsReview }
            };

            var uploaded = new List<RolloverCandidateForValidation>();

            // Act
            var result = _service.ProjectSummary(dbCandidates, uploaded);

            // Assert
            Assert.Equal(3, result.TotalCandidatesCount);
            Assert.Equal(0, result.CandidatesExtendedInUploadCount);
            Assert.Equal(1, result.TotalCandidatesToBeExtendedCount);
            Assert.Equal(1, result.TotalCandidatesToBeExcludedCount);
            Assert.Equal(1, result.TotalCandidatesToBeReviewedCount);
        }


        [Fact]
        public void ProjectSummary_WhenUploadedStatusIsExcluded_CountsCorrectly()
        {
            // Arrange
            var dbCandidates = new List<RolloverCandidateStatusItem>
            {
                new() { Qan = "123", FundingStreamName = "FS1", RolloverStatus = RolloverStatus.NeedsReview }
            };

            var uploaded = new List<RolloverCandidateForValidation>
            {
                new() { Qan = "123", FundingStreamName = "FS1", RollOverStatus = "Excluded", ProposedFundingApprovalEndDate= _defaultEndDate }
            };

            // Act
            var result = _service.ProjectSummary(dbCandidates, uploaded);

            // Assert
            Assert.Equal(1, result.TotalCandidatesToBeExcludedCount);
            Assert.Equal(0, result.TotalCandidatesToBeExtendedCount);
            Assert.Equal(0, result.TotalCandidatesToBeReviewedCount);
        }
    }
}
