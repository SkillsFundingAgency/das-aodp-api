using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Application.UnitTests.Helpers;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;
using Shouldly;

namespace SFA.DAS.AODP.Application.Tests.Commands.Rollover
{
    public class SubmitRolloverExtensionCommandHandlerTests
    {
        private readonly Mock<IRolloverRepository> _rolloverRepository = new();
        private readonly Mock<IRolloverFundingUpdateRepository> _fundingUpdateRepository = new();
        private readonly Mock<ISubmitFundingExtensionService> _applyService = new();
        private readonly Mock<IRolloverFundingEligibilityRepository> _eligibilityRepository = new();
        private readonly Mock<ILogger<SubmitRolloverExtensionCommandHandler>> _logger = new();
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        private readonly SubmitRolloverExtensionCommandHandler _handler;

        public SubmitRolloverExtensionCommandHandlerTests()
        {
            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _handler = new SubmitRolloverExtensionCommandHandler(
                _rolloverRepository.Object,
                _fundingUpdateRepository.Object,
                _applyService.Object,
                _eligibilityRepository.Object,
                _logger.Object);
        }

        private static List<RolloverFundingEligibility> BuildEligibility(
            IEnumerable<RolloverCandidates> candidates, bool isEligible = true)
        {
            return candidates
                .Select(c => new RolloverFundingEligibility(
                    new FundingChangeKey(c.SourceType, c.SourceQualificationId, c.FundingOfferId, c.AcademicYear),
                    c.AcademicYear,
                    null,
                    isEligible))
                .ToList();
        }

        // ------------------------------------------------------------
        // SUCCESS - APPLY EXTENSIONS
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_WhenCandidatesAreMixedOfqualAndQaa_AppliesExtensionsAndPersistsViaService()
        {
            // Arrange
            var item1 = new FundingExtensionItem { Qan = "111", FundingStreamName = "16-18", RolloverStatus = "Extended", ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1) };
            var item2 = new FundingExtensionItem { Qan = "AC1234", FundingStreamName = "19+", RolloverStatus = "Extended", ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(2) };

            var command = new SubmitRolloverExtensionCommand { Items = [item1, item2] };

            var ofqualCandidate = CandidateHelper.BuildCandidate(
                _fixture, item1.Qan, item1.FundingStreamName, Guid.NewGuid(), Guid.NewGuid(), RolloverSourceTypes.Ofqual);
            var qaaCandidate = CandidateHelper.BuildCandidate(
                _fixture, item2.Qan, item2.FundingStreamName, Guid.NewGuid(), Guid.NewGuid(), RolloverSourceTypes.Qaa);
            var rolloverCandidates = new List<RolloverCandidates> { ofqualCandidate, qaaCandidate };

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rolloverCandidates);

            _fundingUpdateRepository
                .Setup(r => r.GetFundingUpdatesAsync(
                    It.IsAny<List<SourceQualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _eligibilityRepository
                .Setup(r => r.GetAsync(It.IsAny<IReadOnlyCollection<FundingChangeKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildEligibility(rolloverCandidates));

            _applyService
                .Setup(s => s.Submit(
                    rolloverCandidates, command.Items, It.IsAny<List<RolloverFundingUpdate>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeTrue();
            result.Value.ResultMessage.ShouldBe("Funding extensions applied.");

            _rolloverRepository.Verify(
                r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never,
                "persistence now happens inside ISubmitFundingExtensionService.Submit via the bulk repository, so the handler must not call SaveChangesAsync itself");
        }

        // ------------------------------------------------------------
        // NO CANDIDATES FOUND
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_WhenNoCandidatesMatch_ReturnsSuccessWithNoMatchMessageAndDoesNotCallDownstreamServices()
        {
            // Arrange
            var command = new SubmitRolloverExtensionCommand
            {
                Items = [new FundingExtensionItem { Qan = "999", FundingStreamName = "16-18", RolloverStatus = "Extended" }]
            };

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeTrue();
            result.Value.ResultMessage.ShouldBe("No matching rollover candidates were found.");

            _fundingUpdateRepository.Verify(
                r => r.GetFundingUpdatesAsync(It.IsAny<List<SourceQualificationFundingKey>>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _applyService.Verify(
                s => s.Submit(It.IsAny<List<RolloverCandidates>>(), It.IsAny<List<FundingExtensionItem>>(),
                    It.IsAny<List<RolloverFundingUpdate>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ------------------------------------------------------------
        // ELIGIBILITY FAILURE
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_WhenACandidateIsNoLongerEligible_ReturnsFailureAndDoesNotCallSubmit()
        {
            // Arrange
            var item = new FundingExtensionItem { Qan = "111", FundingStreamName = "16-18", RolloverStatus = "Extended", ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1) };
            var command = new SubmitRolloverExtensionCommand { Items = [item] };

            var candidate = CandidateHelper.BuildCandidate(
                _fixture, item.Qan, item.FundingStreamName, Guid.NewGuid(), Guid.NewGuid(), RolloverSourceTypes.Ofqual);

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([candidate]);

            _fundingUpdateRepository
                .Setup(r => r.GetFundingUpdatesAsync(
                    It.IsAny<List<SourceQualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _eligibilityRepository
                .Setup(r => r.GetAsync(It.IsAny<IReadOnlyCollection<FundingChangeKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildEligibility([candidate], isEligible: false));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.InnerException.ShouldBeOfType<InvalidOperationException>();

            _applyService.Verify(
                s => s.Submit(It.IsAny<List<RolloverCandidates>>(), It.IsAny<List<FundingExtensionItem>>(),
                    It.IsAny<List<RolloverFundingUpdate>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ------------------------------------------------------------
        // SUBMIT RETURNS FALSE
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_WhenServiceSubmitReturnsFalse_ReturnsSuccessWithFailureMessage()
        {
            // Arrange
            var item = new FundingExtensionItem { Qan = "111", FundingStreamName = "16-18", RolloverStatus = "Extended", ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1) };
            var command = new SubmitRolloverExtensionCommand { Items = [item] };

            var candidate = CandidateHelper.BuildCandidate(
                _fixture, item.Qan, item.FundingStreamName, Guid.NewGuid(), Guid.NewGuid(), RolloverSourceTypes.Ofqual);

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([candidate]);

            _fundingUpdateRepository
                .Setup(r => r.GetFundingUpdatesAsync(
                    It.IsAny<List<SourceQualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _eligibilityRepository
                .Setup(r => r.GetAsync(It.IsAny<IReadOnlyCollection<FundingChangeKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildEligibility([candidate]));

            _applyService
                .Setup(s => s.Submit(
                    It.IsAny<List<RolloverCandidates>>(), command.Items, It.IsAny<List<RolloverFundingUpdate>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert - the handler treats a failed Submit as a soft failure (Success = true,
            // with a message), not a hard error, matching the existing FundingExtensionApplicationException handling
            result.Success.ShouldBeTrue();
            result.Value.ResultMessage.ShouldBe("Failed to apply funding extensions.");
        }

        // ------------------------------------------------------------
        // UNEXPECTED EXCEPTION
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_WhenLoadingCandidatesThrows_ReturnsFailure()
        {
            // Arrange
            var command = new SubmitRolloverExtensionCommand
            {
                Items = [new FundingExtensionItem { Qan = "111", FundingStreamName = "16-18", RolloverStatus = "Extended" }]
            };

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database unavailable."));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldBe("Database unavailable.");
        }
    }
}