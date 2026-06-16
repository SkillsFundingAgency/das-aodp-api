using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Application.Services.Validation;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Tests.Commands.Rollover
{
    public class ValidateFundingExtensionCandidatesCommandHandlerTests
    {
        private readonly Mock<IRolloverRepository> _rolloverRepository = new();
        private readonly Mock<IRolloverFundingExtensionValidator> _validator = new();
        private readonly Mock<IFundingExtensionCandidatesCsvBuilder> _csvBuilder = new();
        private readonly Mock<IFundingExtensionProjectionService> _projectionService = new();

        private readonly ValidateFundingExtensionCandidatesCommandHandler _handler;

        public ValidateFundingExtensionCandidatesCommandHandlerTests()
        {
            _handler = new ValidateFundingExtensionCandidatesCommandHandler(
                _rolloverRepository.Object,
                _validator.Object,
                _csvBuilder.Object,
                _projectionService.Object);
        }

        // ------------------------------------------------------------
        // SUCCESS PATH — VALIDATION PASSES → PROJECTION SERVICE CALLED
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_Success_UsesProjectionServiceAndReturnsSummary()
        {
            // Arrange
            var command = new ValidateFundingExtensionCandidatesCommand
            {
                FundingExtensionCandidates =
                [
                    new() { Qan = "111", FundingStreamName = "FS", RollOverStatus = "To Extend" },
                    new() { Qan = "222", FundingStreamName = "FS", RollOverStatus = "To Exclude" }
                ]
            };

            var incomingKeys = new HashSet<CandidateKey>
            {
                new("111", "FS"),
                new("222", "FS")
            };

            var fakeContext = new FundingExtensionCandidateValidationContext(
                incomingKeys, incomingKeys, incomingKeys);

            _rolloverRepository
                .Setup(r => r.GetFundingExtensionValidationContextAsync(
                    It.IsAny<HashSet<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeContext);

            var validationResult = new FundingExtensionValidationResult
            {
                IsValid = true,
                Candidates =
                [
                    new CandidateValidationResult { CandidateDetails = command.FundingExtensionCandidates[0] },
                    new CandidateValidationResult { CandidateDetails = command.FundingExtensionCandidates[1] }
                ]
            };

            _validator
                .Setup(v => v.Validate(
                    command.FundingExtensionCandidates, fakeContext, It.IsAny<CancellationToken>()))
                .Returns(validationResult);

            var dbCandidates = new List<FundingExtensionCandidateItem>
            {
                new() { Qan = "111", FundingStreamName = "FS", RolloverStatus = RolloverStatus.NeedsReview },
                new() { Qan = "222", FundingStreamName = "FS", RolloverStatus = RolloverStatus.NeedsReview },
                new() { Qan = "333", FundingStreamName = "FS", RolloverStatus = RolloverStatus.NeedsReview }
            };

            _rolloverRepository
                .Setup(r => r.GetFundingExtensionCandidatesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(dbCandidates);

            var expectedSummary = new FundingExtensionSummary
            {
                TotalCandidatesCount = 3,
                CandidatesExtendedInUploadCount = 2,
                TotalCandidatesToBeExtendedCount = 1,
                TotalCandidatesToBeExcludedCount = 1,
                TotalCandidatesToBeReviewedCount = 1
            };

            _projectionService
                .Setup(p => p.ProjectSummary(dbCandidates, It.IsAny<List<FundingExtensionCandidate>>()))
                .Returns(expectedSummary);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Value.IsValid);
            Assert.Equal(expectedSummary, result.Value.ValidationSuccessSummary);

            _projectionService.Verify(
                p => p.ProjectSummary(dbCandidates, It.IsAny<List<FundingExtensionCandidate>>()),
                Times.Once);
        }

        // ------------------------------------------------------------
        // FAILURE PATH — VALIDATION FAILS → CSV BUILDER CALLED
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_InvalidValidation_BuildsCsvAndReturnsFile()
        {
            // Arrange
            var command = new ValidateFundingExtensionCandidatesCommand
            {
                FundingExtensionCandidates =
                [
                    new() { Qan = "111", FundingStreamName = "FS", RollOverStatus = "To Extend" }
                ]
            };

            var incomingKeys = new HashSet<CandidateKey> { new("111", "FS") };

            var fakeContext = new FundingExtensionCandidateValidationContext(
                incomingKeys, incomingKeys, incomingKeys);

            _rolloverRepository
                .Setup(r => r.GetFundingExtensionValidationContextAsync(
                    It.IsAny<HashSet<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeContext);

            var validationResult = new FundingExtensionValidationResult
            {
                IsValid = false,
                FailedCandidateCount = 1,
                Candidates =
                [
                    new CandidateValidationResult
                    {
                        CandidateDetails = command.FundingExtensionCandidates[0],
                        Errors = [ new ValidationFailure { Field = "QAN", Message = "Invalid QAN" } ]
                    }
                ]
            };

            _validator
                .Setup(v => v.Validate(
                    command.FundingExtensionCandidates, fakeContext, It.IsAny<CancellationToken>()))
                .Returns(validationResult);

            var csvBytes = new byte[] { 1, 2, 3 };

            _csvBuilder
                .Setup(x => x.BuildWithValidationErrors(
                    It.IsAny<List<FundingExtensionCandidateDto>>(),
                    validationResult.Candidates))
                .Returns(csvBytes);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.False(result.Value.IsValid);
            Assert.Equal(csvBytes, result.Value.ValidationFailureSummary.ValidatedCandidateFile);

            _csvBuilder.Verify(
                x => x.BuildWithValidationErrors(
                    It.IsAny<List<FundingExtensionCandidateDto>>(),
                    validationResult.Candidates),
                Times.Once);

            _projectionService.Verify(
                p => p.ProjectSummary(It.IsAny<List<FundingExtensionCandidateItem>>(),
                                      It.IsAny<List<FundingExtensionCandidate>>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_GenericException_ReturnsErrorMessage()
        {
            var command = new ValidateFundingExtensionCandidatesCommand
            {
                FundingExtensionCandidates = []
            };

            _rolloverRepository
                .Setup(r => r.GetFundingExtensionValidationContextAsync(
                    It.IsAny<HashSet<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Exception happened"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Exception happened", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_NoWorkflowRunsExist_ReturnsFailure()
        {
            var command = new ValidateFundingExtensionCandidatesCommand
            {
                FundingExtensionCandidates =
                [
                    new() { Qan = "60110314", FundingStreamName = "FS", RollOverStatus = "To Extend" }
                ]
            };

            _rolloverRepository
                .Setup(r => r.GetFundingExtensionValidationContextAsync(
                    It.IsAny<HashSet<CandidateKey>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("No workflow runs exist"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Contains("No workflow runs exist", result.ErrorMessage);
        }
    }
}
