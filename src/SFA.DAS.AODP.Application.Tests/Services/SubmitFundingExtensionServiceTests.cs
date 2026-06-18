using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Application.UnitTests.Helpers;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Tests.Services.Rollover
{
    public class SubmitFundingExtensionServiceTests
    {
        private readonly Mock<IRolloverRepository> _rolloverRepository = new();
        private readonly Mock<IQualificationDiscussionHistoryRepository> _historyRepository = new();
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        private readonly SubmitFundingExtensionService _service;

        public SubmitFundingExtensionServiceTests()
        {
            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _service = new SubmitFundingExtensionService(
                _rolloverRepository.Object,
                _historyRepository.Object);
        }

        // ------------------------------------------------------------
        // SUCCESS — EXTENDED STATUS UPDATES FUNDING + CANDIDATE
        // ------------------------------------------------------------
        [Fact]
        public async Task Submit_Extended_UpdatesCandidateAndFunding_AndCreatesHistory()
        {
            // Arrange
            var item = new FundingExtensionItem
            {
                Qan = "111",
                FundingStreamName = "FS",
                RolloverStatus = "Extended",
                ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1),
                Comments = "Test comment"
            };

            var candidate = CandidateHelper.BuildCandidate(_fixture, item.Qan, item.FundingStreamName);
            var funding = new QualificationFundings
            {
                QualificationVersionId = candidate.QualificationVersionId,
                FundingOfferId = candidate.FundingOfferId
            };

            var candidates = new List<RolloverCandidates> { candidate };
            var fundings = new List<QualificationFundings> { funding };

            // Act
            var result = await _service.Submit(candidates, [item], fundings, CancellationToken.None);

            // Assert
            Assert.True(result);

            Assert.Equal(RolloverStatus.Extended, candidate.RolloverStatus);
            Assert.Equal(item.ProposedFundingApprovalEndDate.Date, funding.EndDate?.ToDateTime(TimeOnly.MinValue).Date);
            Assert.Equal(item.Comments, funding.Comments);

            _historyRepository.Verify(h =>
                h.AddDiscussionHistories(It.IsAny<List<QualificationDiscussionHistory>>()),
                Times.Once);

            _rolloverRepository.Verify(r => r.DeleteAllWorkflowCandidatesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // ------------------------------------------------------------
        // SUCCESS — EXCLUDED STATUS UPDATES CANDIDATE + HISTORY
        // ------------------------------------------------------------
        [Fact]
        public async Task Submit_Excluded_SetsExcluded_AndCreatesHistory()
        {
            // Arrange
            var item = new FundingExtensionItem
            {
                Qan = "111",
                FundingStreamName = "FS",
                RolloverStatus = "Excluded",
                ExclusionReason = "Bad data"
            };

            var candidate = CandidateHelper.BuildCandidate(_fixture, item.Qan, item.FundingStreamName);
            var funding = new QualificationFundings
            {
                QualificationVersionId = candidate.QualificationVersionId,
                FundingOfferId = candidate.FundingOfferId
            };

            var candidates = new List<RolloverCandidates> { candidate };
            var fundings = new List<QualificationFundings> { funding };

            // Act
            var result = await _service.Submit(candidates, [item], fundings, CancellationToken.None);

            // Assert
            Assert.True(result);

            Assert.Equal(RolloverStatus.Excluded, candidate.RolloverStatus);
            Assert.Equal("Bad data", candidate.ExclusionReason);

            _historyRepository.Verify(h =>
                h.AddDiscussionHistories(It.Is<List<QualificationDiscussionHistory>>(l => l.Count == 1)),
                Times.Once);
        }

        // ------------------------------------------------------------
        // MIXED — EXTENDED + EXCLUDED → TWO HISTORY ENTRIES
        // ------------------------------------------------------------
        [Fact]
        public async Task Submit_MixedStatuses_CreatesTwoHistoryEntries()
        {
            // Arrange
            var item1 = new FundingExtensionItem
            {
                Qan = "111",
                FundingStreamName = "FS1",
                RolloverStatus = "Extended",
                ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1)
            };

            var item2 = new FundingExtensionItem
            {
                Qan = "222",
                FundingStreamName = "FS2",
                RolloverStatus = "Excluded",
                ExclusionReason = "Reason"
            };

            var candidate1 = CandidateHelper.BuildCandidate(_fixture, item1.Qan, item1.FundingStreamName);
            var candidate2 = CandidateHelper.BuildCandidate(_fixture, item2.Qan, item2.FundingStreamName);

            // Same qualification → grouped together
            candidate2.QualificationVersion.QualificationId = candidate1.QualificationVersion.QualificationId;

            var funding1 = new QualificationFundings
            {
                QualificationVersionId = candidate1.QualificationVersionId,
                FundingOfferId = candidate1.FundingOfferId
            };

            var candidates = new List<RolloverCandidates> { candidate1, candidate2 };
            var fundings = new List<QualificationFundings> { funding1 };

            // Act
            var result = await _service.Submit(candidates, [item1, item2], fundings, CancellationToken.None);

            // Assert
            Assert.True(result);

            _historyRepository.Verify(h =>
                h.AddDiscussionHistories(It.Is<List<QualificationDiscussionHistory>>(l => l.Count == 2)),
                Times.Once);
        }

        // ------------------------------------------------------------
        // NO MATCHING INPUT → NO UPDATES, NO HISTORY
        // ------------------------------------------------------------
        [Fact]
        public async Task Submit_NoMatchingInput_DoesNothing_ReturnsTrue()
        {
            // Arrange
            var candidate = CandidateHelper.BuildCandidate(_fixture, "111", "FS");
            var funding = new QualificationFundings
            {
                QualificationVersionId = candidate.QualificationVersionId,
                FundingOfferId = candidate.FundingOfferId
            };

            // Input does not match candidate
            var item = new FundingExtensionItem
            {
                Qan = "XXX",
                FundingStreamName = "YYY",
                RolloverStatus = "Extended"
            };

            // Act
            var result = await _service.Submit([candidate], [item], [funding], CancellationToken.None);

            // Assert
            Assert.True(result);

            Assert.Equal(RolloverStatus.NeedsReview, candidate.RolloverStatus);

            _historyRepository.Verify(h => h.AddDiscussionHistories(It.IsAny<List<QualificationDiscussionHistory>>()), Times.Once);
        }

        // ------------------------------------------------------------
        // EXCEPTION → RETURNS FALSE
        // ------------------------------------------------------------
        [Fact]
        public async Task Submit_ExceptionThrown_ReturnsFalse()
        {
            // Arrange
            var candidate = CandidateHelper.BuildCandidate(_fixture, "111", "FS");
            var funding = new QualificationFundings
            {
                QualificationVersionId = candidate.QualificationVersionId,
                FundingOfferId = candidate.FundingOfferId
            };

            _historyRepository
                .Setup(h => h.AddDiscussionHistories(It.IsAny<List<QualificationDiscussionHistory>>()))
                .ThrowsAsync(new Exception("Boom"));

            // Act
            var result = await _service.Submit([candidate], [], [funding], CancellationToken.None);

            // Assert
            Assert.False(result);
        }

    }
}
