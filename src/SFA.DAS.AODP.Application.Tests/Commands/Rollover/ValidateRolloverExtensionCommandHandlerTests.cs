using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Application.Services.FundingExtension;
using SFA.DAS.AODP.Application.Services.Validation;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Tests.Commands.Rollover
{
    public class ValidateRolloverExtensionCommandHandlerTests
    {
        private readonly Mock<IRolloverRepository> _rolloverRepository = new();
        private readonly Mock<IRolloverFundingExtensionValidator> _validator = new();
        private readonly Mock<IFundingExtensionCandidatesCsvBuilder> _csvBuilder = new();
        private readonly Mock<IFundingExtensionProjectionService> _projectionService = new();

        private readonly ValidateRolloverExtensionCommandHandler _handler;

        public ValidateRolloverExtensionCommandHandlerTests()
        {
            _handler = new ValidateRolloverExtensionCommandHandler(
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
            var command = new ValidateRolloverExtensionCommand
            {
                RolloverCandidates =
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
                    new CandidateValidationResult { CandidateDetails = command.RolloverCandidates[0] },
                    new CandidateValidationResult { CandidateDetails = command.RolloverCandidates[1] }
                ]
            };

            _validator
                .Setup(v => v.Validate(
                    command.RolloverCandidates, fakeContext, It.IsAny<CancellationToken>()))
                .Returns(validationResult);

            var dbCandidates = new List<RolloverCandidateStatusItem>
            {
                new() { Qan = "111", FundingStreamName = "FS", RolloverStatus = RolloverStatus.NeedsReview },
                new() { Qan = "222", FundingStreamName = "FS", RolloverStatus = RolloverStatus.NeedsReview },
                new() { Qan = "333", FundingStreamName = "FS", RolloverStatus = RolloverStatus.NeedsReview }
            };

            _rolloverRepository
                .Setup(r => r.GetRolloverCandidatesStatusAsync(It.IsAny<CancellationToken>()))
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
                .Setup(p => p.ProjectSummary(dbCandidates, It.IsAny<List<RolloverCandidateForValidation>>()))
                .Returns(expectedSummary);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Value.IsValid);
            Assert.Equal(expectedSummary, result.Value.ValidationSuccessSummary);

            _projectionService.Verify(
                p => p.ProjectSummary(dbCandidates, It.IsAny<List<RolloverCandidateForValidation>>()),
                Times.Once);
        }

        // ------------------------------------------------------------
        // FAILURE PATH — VALIDATION FAILS → CSV BUILDER CALLED
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_InvalidValidation_BuildsCsvAndReturnsFile()
        {
            // Arrange
            var command = new ValidateRolloverExtensionCommand
            {
                RolloverCandidates =
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

            _rolloverRepository
                .Setup(r => r.GetLatestWorkflowRunIdAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            var validationResult = new FundingExtensionValidationResult
            {
                IsValid = false,
                FailedCandidateCount = 1,
                Candidates =
                [
                    new CandidateValidationResult
            {
                CandidateDetails = command.RolloverCandidates[0],
                Errors = [ new ValidationFailure { Field = "QAN", Message = "Invalid QAN" } ]
            }
                ]
            };

            _validator
                .Setup(v => v.Validate(
                    command.RolloverCandidates, fakeContext, It.IsAny<CancellationToken>()))
                .Returns(validationResult);

            var exportRows = new List<RolloverCandidateForExport>
            {
                new()
                {
                    QAN = "111",
                    FundingStreamName = "FS",
                }
            };

            _rolloverRepository
                .Setup(r => r.GetRolloverWorkflowCandidatesByRunId(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(exportRows);

            var csvBytes = new byte[] { 1, 2, 3 };

            _csvBuilder
                .Setup(x => x.BuildWithValidationErrors(
                    exportRows,
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
                    exportRows,
                    validationResult.Candidates),
                Times.Once);

            _projectionService.Verify(
                p => p.ProjectSummary(It.IsAny<List<RolloverCandidateStatusItem>>(),
                                      It.IsAny<List<RolloverCandidateForValidation>>()),
                Times.Never);
        }


        [Fact]
        public async Task Handle_GenericException_ReturnsErrorMessage()
        {
            var command = new ValidateRolloverExtensionCommand
            {
                RolloverCandidates = []
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
            var command = new ValidateRolloverExtensionCommand
            {
                RolloverCandidates =
                [
                    new() { Qan = "60110314", FundingStreamName = "FS", RollOverStatus = "Extended" }
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
