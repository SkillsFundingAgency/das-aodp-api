using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Application.UnitTests.Helpers;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Tests.Commands.Rollover
{
    public class SubmitRolloverExtensionCommandHandlerTests
    {
        private readonly Mock<IRolloverRepository> _rolloverRepository = new();
        private readonly Mock<IRolloverFundingUpdateRepository> _fundingUpdateRepository = new();
        private readonly Mock<ISubmitFundingExtensionService> _applyService = new();
        private readonly Mock<IFundingChangeCoordinator> _fundingChangeCoordinator = new();
        private readonly Mock<IRolloverFundingEligibilityRepository> _fundingEligibilityRepository = new();
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

        private readonly SubmitRolloverExtensionCommandHandler _handler;

        public SubmitRolloverExtensionCommandHandlerTests()
        {
            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _fundingChangeCoordinator
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<FundingChangeSet>(),
                    It.IsAny<Func<CancellationToken, Task<bool>>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((FundingChangeSet _, Func<CancellationToken, Task<bool>> mutation, CancellationToken ct) =>
                    mutation(ct));

            _fundingEligibilityRepository
                .Setup(x => x.GetAsync(
                    It.IsAny<IReadOnlyCollection<FundingChangeKey>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((
                    IReadOnlyCollection<FundingChangeKey> keys,
                    CancellationToken _) => keys
                    .Select(x => new RolloverFundingEligibility(
                        x,
                        x.AcademicYear!,
                        null,
                        true))
                    .ToList());

            _handler = new SubmitRolloverExtensionCommandHandler(
                _rolloverRepository.Object,
                _fundingUpdateRepository.Object,
                _applyService.Object,
                _fundingChangeCoordinator.Object,
                _fundingEligibilityRepository.Object);
        }

        // ------------------------------------------------------------
        // SUCCESS — APPLY EXTENSIONS → SAVE CHANGES
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_Success_AppliesExtensionsAndSavesChanges()
        {
            // Arrange
            var item1 = new FundingExtensionItem() { Qan = "111", FundingStreamName = "16-18", RolloverStatus = "Extended", ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1) };
            var item2 = new FundingExtensionItem() { Qan = "222", FundingStreamName = "19+", RolloverStatus = "Extended", ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(2) };

            var command = new SubmitRolloverExtensionCommand
            {
                Items = [item1, item2]
                                   
            };

            var candidate1 = CandidateHelper.BuildCandidate(_fixture, item1.Qan, item1.FundingStreamName);
            var candidate2 = CandidateHelper.BuildCandidate(_fixture, item2.Qan, item2.FundingStreamName);
            var rolloverCandidates = new List<RolloverCandidates> { candidate1, candidate2 };

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(rolloverCandidates);

            var fundingUpdates = new List<RolloverFundingUpdate>
            {
                BuildFundingUpdate(candidate1),
                BuildFundingUpdate(candidate2)
            };

            _fundingUpdateRepository
                .Setup(r => r.GetFundingUpdatesAsync(
                    It.IsAny<List<SourceQualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fundingUpdates);

            _applyService
                .Setup(s => s.Submit(
                    rolloverCandidates, command.Items, fundingUpdates, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Funding extensions applied.", result.Value.ResultMessage);

            _fundingChangeCoordinator.Verify(r => r.ExecuteAsync(
                It.IsAny<FundingChangeSet>(),
                It.IsAny<Func<CancellationToken, Task<bool>>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoMatchingCandidates_ReturnsSuccessWithMessage()
        {
            // Arrange
            var command = new SubmitRolloverExtensionCommand
            {
                Items =
                [
                    new() { Qan = "999", FundingStreamName = "FS", RolloverStatus = "Extended" }
                ]
            };

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RolloverCandidates>()); // no matches

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("No matching rollover candidates were found.", result.Value.ResultMessage);

            _applyService.Verify(
                s => s.Submit(
                    It.IsAny<List<RolloverCandidates>>(),
                    It.IsAny<List<FundingExtensionItem>>(),
                    It.IsAny<List<RolloverFundingUpdate>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ApplyFundingExtensionsFails_ReturnsFailureMessage()
        {
            // Arrange
            var item = new FundingExtensionItem
            {
                Qan = "111",
                FundingStreamName = "FS",
                RolloverStatus = "Extended",
                ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1)
            };

            var command = new SubmitRolloverExtensionCommand
            {
                Items = [item]
            };

            var candidate = CandidateHelper.BuildCandidate(_fixture, item.Qan, item.FundingStreamName);
            var candidates = new List<RolloverCandidates> { candidate };

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            var fundingUpdates = new List<RolloverFundingUpdate>
            {
                BuildFundingUpdate(candidate)
            };

            _fundingUpdateRepository
                .Setup(r => r.GetFundingUpdatesAsync(
                    It.IsAny<List<SourceQualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fundingUpdates);

            _applyService
                .Setup(s => s.Submit(
                    candidates, command.Items, fundingUpdates, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Failed to apply funding extensions.", result.Value.ResultMessage);

            _rolloverRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_CandidateNoLongerEligible_RejectsSubmission()
        {
            var item = new FundingExtensionItem
            {
                Qan = "111",
                FundingStreamName = "FS",
                RolloverStatus = "Extended"
            };
            var candidate = CandidateHelper.BuildCandidate(
                _fixture,
                item.Qan,
                item.FundingStreamName);
            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([candidate]);
            _fundingEligibilityRepository
                .Setup(x => x.GetAsync(
                    It.IsAny<IReadOnlyCollection<FundingChangeKey>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([
                    new RolloverFundingEligibility(
                        new FundingChangeKey(
                            candidate.SourceType,
                            candidate.SourceQualificationId,
                            candidate.FundingOfferId,
                            candidate.AcademicYear),
                        candidate.AcademicYear,
                        null,
                        false)
                ]);

            var result = await _handler.Handle(
                new SubmitRolloverExtensionCommand { Items = [item] },
                TestContext.Current.CancellationToken);

            Assert.False(result.Success);
            Assert.Equal(
                "One or more rollover candidates are no longer backed by applicable funding.",
                result.ErrorMessage);
            _fundingChangeCoordinator.Verify(x => x.ExecuteAsync(
                It.IsAny<FundingChangeSet>(),
                It.IsAny<Func<CancellationToken, Task<bool>>>(),
                It.IsAny<CancellationToken>()), Times.Once);
            _applyService.Verify(x => x.Submit(
                It.IsAny<List<RolloverCandidates>>(),
                It.IsAny<List<FundingExtensionItem>>(),
                It.IsAny<List<RolloverFundingUpdate>>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExceptionThrown_ReturnsErrorMessage()
        {
            // Arrange
            var command = new SubmitRolloverExtensionCommand
            {
                Items = [new() { Qan = "111", FundingStreamName = "FS", RolloverStatus = "Extended" }]
            };

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Boom"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Boom", result.ErrorMessage);
            Assert.IsType<Exception>(result.InnerException);
        }

        [Fact]
        public async Task Handle_PassesCorrectCandidateKeysToRepository()
        {
            // Arrange
            var command = new SubmitRolloverExtensionCommand
            {
                Items =
                [
                    new() { Qan = "A1", FundingStreamName = "FS1", RolloverStatus = "Extended" },
                    new() { Qan = "B2", FundingStreamName = "FS2", RolloverStatus = "Extended" }
                ]
            };

            List<CandidateKey>? capturedKeys = null;

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .Callback<List<CandidateKey>, CancellationToken>((keys, _) => capturedKeys = keys)
                .ReturnsAsync(new List<RolloverCandidates>());

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedKeys);
            Assert.Contains(capturedKeys!, k => k.Qan == "A1" && k.FundingStream == "FS1");
            Assert.Contains(capturedKeys!, k => k.Qan == "B2" && k.FundingStream == "FS2");
        }

        [Fact]
        public async Task Handle_PassesCorrectFundingKeysToRepository()
        {
            // Arrange
            var item = new FundingExtensionItem
            {
                Qan = "111",
                FundingStreamName = "FS",
                RolloverStatus = "Extended",
                ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1)
            };

            var command = new SubmitRolloverExtensionCommand
            {
                Items = [item]
            };

            var candidate = CandidateHelper.BuildCandidate(_fixture, item.Qan, item.FundingStreamName);
            var candidates = new List<RolloverCandidates> { candidate };

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            List<SourceQualificationFundingKey>? capturedKeys = null;

            _fundingUpdateRepository
                .Setup(r => r.GetFundingUpdatesAsync(
                    It.IsAny<List<SourceQualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .Callback<List<SourceQualificationFundingKey>, CancellationToken>((keys, _) => capturedKeys = keys)
                .ReturnsAsync([]);

            _applyService
                .Setup(s => s.Submit(
                    candidates, command.Items, It.IsAny<List<RolloverFundingUpdate>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedKeys);
            Assert.Single(capturedKeys!);

            var key = capturedKeys!.First();
            Assert.Equal(candidate.SourceType, key.SourceType);
            Assert.Equal(candidate.SourceQualificationId, key.SourceQualificationId);
            Assert.Equal(candidate.FundingOfferId, key.FundingOfferId);
        }

        [Fact]
        public async Task Handle_CallsApplyFundingExtensionsWithCorrectArguments()
        {
            // Arrange
            var item = new FundingExtensionItem
            {
                Qan = "111",
                FundingStreamName = "FS",
                RolloverStatus = "Extended",
                ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1)
            };

            var command = new SubmitRolloverExtensionCommand
            {
                Items = [item]
            };

            var candidate = CandidateHelper.BuildCandidate(_fixture, item.Qan, item.FundingStreamName);
            var candidates = new List<RolloverCandidates> { candidate };

            _rolloverRepository
                .Setup(r => r.LoadRolloverCandidateGraphAsync(
                    It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            var fundingUpdates = new List<RolloverFundingUpdate>
            {
                BuildFundingUpdate(candidate)
            };

            _fundingUpdateRepository
                .Setup(r => r.GetFundingUpdatesAsync(
                    It.IsAny<List<SourceQualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fundingUpdates);

            _applyService
                .Setup(s => s.Submit(
                    It.IsAny<List<RolloverCandidates>>(),
                    It.IsAny<List<FundingExtensionItem>>(),
                    It.IsAny<List<RolloverFundingUpdate>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _applyService.Verify(s =>
                s.Submit(
                    It.Is<List<RolloverCandidates>>(l => l.SequenceEqual(candidates)),
                    It.Is<List<FundingExtensionItem>>(l => l.SequenceEqual(command.Items)),
                    It.Is<List<RolloverFundingUpdate>>(l => l.SequenceEqual(fundingUpdates)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static RolloverFundingUpdate BuildFundingUpdate(RolloverCandidates candidate) =>
            new(
                candidate.SourceType,
                candidate.SourceQualificationId,
                candidate.FundingOfferId,
                candidate.AcademicYear,
                null,
                (_, _, _) => { });
    } 
}
