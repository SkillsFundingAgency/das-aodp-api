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

namespace SFA.DAS.AODP.Application.Tests.Commands.Rollover
{
    public class SubmitRolloverExtensionCommandHandlerTests
    {
        private readonly Mock<IRolloverRepository> _rolloverRepository = new();
        private readonly Mock<IQualificationFundingsRepository> _fundingsRepository = new();
        private readonly Mock<ISubmitFundingExtensionService> _applyService = new();
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
                _fundingsRepository.Object,
                _applyService.Object);
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

            var fundings = new List<QualificationFundings>
            {
                new () { QualificationVersionId = candidate1.QualificationVersionId , FundingOfferId = candidate1.FundingOfferId },
                new() { QualificationVersionId = candidate2.QualificationVersionId , FundingOfferId = candidate2.FundingOfferId }
            };

            _fundingsRepository
                .Setup(r => r.GetRolloverQualificationFundingsAsync(
                    It.IsAny<List<QualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fundings);

            _applyService
                .Setup(s => s.Submit(
                    rolloverCandidates, command.Items, fundings, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Funding extensions applied.", result.Value.ResultMessage);

            _rolloverRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoMatchingCandidates_ReturnsSuccessWithMessage()
        {
            // Arrange
            var command = new SubmitRolloverExtensionCommand
            {
                Items =
                [
                    new() { Qan = "999", FundingStreamName = "FS" }
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
                    It.IsAny<List<QualificationFundings>>(),
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

            var fundings = new List<QualificationFundings>
            {
                new() { QualificationVersionId = candidate.QualificationVersionId, FundingOfferId = candidate.FundingOfferId }
            };

            _fundingsRepository
                .Setup(r => r.GetRolloverQualificationFundingsAsync(
                    It.IsAny<List<QualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fundings);

            _applyService
                .Setup(s => s.Submit(
                    candidates, command.Items, fundings, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Failed to apply funding extensions.", result.Value.ResultMessage);

            _rolloverRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExceptionThrown_ReturnsErrorMessage()
        {
            // Arrange
            var command = new SubmitRolloverExtensionCommand
            {
                Items = [new() { Qan = "111", FundingStreamName = "FS" }]
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
                    new() { Qan = "A1", FundingStreamName = "FS1" },
                    new() { Qan = "B2", FundingStreamName = "FS2" }
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

            List<QualificationFundingKey>? capturedKeys = null;

            _fundingsRepository
                .Setup(r => r.GetRolloverQualificationFundingsAsync(
                    It.IsAny<List<QualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .Callback<List<QualificationFundingKey>, CancellationToken>((keys, _) => capturedKeys = keys)
                .ReturnsAsync(new List<QualificationFundings>());

            _applyService
                .Setup(s => s.Submit(
                    candidates, command.Items, It.IsAny<List<QualificationFundings>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedKeys);
            Assert.Single(capturedKeys!);

            var key = capturedKeys!.First();
            Assert.Equal(candidate.QualificationVersionId, key.QualificationVersionId);
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

            var fundings = new List<QualificationFundings>
            {
                new() { QualificationVersionId = candidate.QualificationVersionId, FundingOfferId = candidate.FundingOfferId }
            };

            _fundingsRepository
                .Setup(r => r.GetRolloverQualificationFundingsAsync(
                    It.IsAny<List<QualificationFundingKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fundings);

            _applyService
                .Setup(s => s.Submit(
                    It.IsAny<List<RolloverCandidates>>(),
                    It.IsAny<List<FundingExtensionItem>>(),
                    It.IsAny<List<QualificationFundings>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _applyService.Verify(s =>
                s.Submit(
                    It.Is<List<RolloverCandidates>>(l => l.SequenceEqual(candidates)),
                    It.Is<List<FundingExtensionItem>>(l => l.SequenceEqual(command.Items)),
                    It.Is<List<QualificationFundings>>(l => l.SequenceEqual(fundings)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        
    }

    //// ------------------------------------------------------------
    //// SUCCESS — NO MATCHING CANDIDATES
    //// ------------------------------------------------------------
    //[Fact]
    //public async Task Handle_NoMatchingCandidates_ReturnsSuccessWithMessage()
    //{
    //    // Arrange
    //    var command = new SubmitRolloverExtensionCommand
    //    {
    //        Items = [new() { Qan = "999", FundingStreamName = "FS" }]
    //    };

    //    _rolloverRepository
    //        .Setup(r => r.LoadRolloverCandidateGraphAsync(
    //            It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(new List<RolloverCandidate>());

    //    // Act
    //    var result = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    Assert.True(result.Success);
    //    Assert.Equal("No matching rollover candidates were found.", result.Value.ResultMessage);

    //    _applyService.Verify(s => s.ApplyFundingExtensions(
    //        It.IsAny<List<RolloverCandidate>>(),
    //        It.IsAny<List<SubmitRolloverExtensionItem>>(),
    //        It.IsAny<List<QualificationFunding>>(),
    //        It.IsAny<CancellationToken>()),
    //        Times.Never);
    //}

    //// ------------------------------------------------------------
    //// FAILURE — APPLY EXTENSIONS RETURNS FALSE
    //// ------------------------------------------------------------
    //[Fact]
    //public async Task Handle_ApplyFundingExtensionsFails_ReturnsFailureMessage()
    //{
    //    // Arrange
    //    var command = new SubmitRolloverExtensionCommand
    //    {
    //        Items = [new() { Qan = "111", FundingStreamName = "FS" }]
    //    };

    //    var candidates = new List<RolloverCandidate>
    //    {
    //        new() { Qan = "111", FundingStreamName = "FS", QualificationVersionId = 1, FundingOfferId = 10 }
    //    };

    //    _rolloverRepository
    //        .Setup(r => r.LoadRolloverCandidateGraphAsync(
    //            It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(candidates);

    //    _fundingsRepository
    //        .Setup(r => r.GetRolloverQualificationFundingsAsync(
    //            It.IsAny<List<QualificationFundingKey>>(), It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(new List<QualificationFunding>());

    //    _applyService
    //        .Setup(s => s.ApplyFundingExtensions(
    //            candidates, command.Items, It.IsAny<List<QualificationFunding>>(), It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(false);

    //    // Act
    //    var result = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    Assert.True(result.Success);
    //    Assert.Equal("Failed to apply funding extensions.", result.Value.ResultMessage);

    //    _rolloverRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    //}

    //// ------------------------------------------------------------
    //// EXCEPTION — RETURNS FAILURE RESPONSE
    //// ------------------------------------------------------------
    //[Fact]
    //public async Task Handle_ExceptionThrown_ReturnsErrorMessage()
    //{
    //    // Arrange
    //    var command = new SubmitRolloverExtensionCommand
    //    {
    //        Items = [new() { Qan = "111", FundingStreamName = "FS" }]
    //    };

    //    _rolloverRepository
    //        .Setup(r => r.LoadRolloverCandidateGraphAsync(
    //            It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
    //        .ThrowsAsync(new Exception("Boom"));

    //    // Act
    //    var result = await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    Assert.False(result.Success);
    //    Assert.Equal("Boom", result.ErrorMessage);
    //    Assert.IsType<Exception>(result.InnerException);
    //}

    //// ------------------------------------------------------------
    //// ARGUMENT VERIFICATION — CORRECT KEYS PASSED TO REPOSITORY
    //// ------------------------------------------------------------
    //[Fact]
    //public async Task Handle_PassesCorrectCandidateKeysToRepository()
    //{
    //    // Arrange
    //    var command = new SubmitRolloverExtensionCommand
    //    {
    //        Items =
    //        [
    //            new() { Qan = "A1", FundingStreamName = "FS1" },
    //            new() { Qan = "B2", FundingStreamName = "FS2" }
    //        ]
    //    };

    //    List<CandidateKey>? capturedKeys = null;

    //    _rolloverRepository
    //        .Setup(r => r.LoadRolloverCandidateGraphAsync(
    //            It.IsAny<List<CandidateKey>>(), It.IsAny<CancellationToken>()))
    //        .Callback<List<CandidateKey>, CancellationToken>((keys, _) => capturedKeys = keys)
    //        .ReturnsAsync(new List<RolloverCandidate>());

    //    // Act
    //    await _handler.Handle(command, CancellationToken.None);

    //    // Assert
    //    Assert.NotNull(capturedKeys);
    //    Assert.Contains(capturedKeys!, k => k.Qan == "A1" && k.FundingStreamName == "FS1");
    //    Assert.Contains(capturedKeys!, k => k.Qan == "B2" && k.FundingStreamName == "FS2");
    //}
}
