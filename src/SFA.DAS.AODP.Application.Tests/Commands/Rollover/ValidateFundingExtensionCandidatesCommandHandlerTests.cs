using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Application.Services.Validation;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Tests.Commands.Rollover
{
    public class ValidateFundingExtensionCandidatesCommandHandlerTests
    {
        private readonly Mock<IRolloverRepository> _rolloverRepository = new();
        private readonly Mock<IRolloverFundingExtensionValidator> _validator = new();
        private readonly ValidateFundingExtensionCandidatesCommandHandler _handler;
        private readonly Mock<IFundingExtensionCandidatesCsvBuilder> _csvBuilder = new();

        public ValidateFundingExtensionCandidatesCommandHandlerTests()
        {
            _handler = new ValidateFundingExtensionCandidatesCommandHandler(
                _rolloverRepository.Object,
                _validator.Object,
                _csvBuilder.Object);
        }

        [Fact]
        public async Task Handle_Success_ReturnsMappedValidatorResult()
        {
            // Arrange
            var command = new ValidateFundingExtensionCandidatesCommand
            {
                FundingExtensionCandidates = new List<FundingExtensionCandidate>
        {
            new()
            {
                RowNumber = 1,
                Qan = "60110314",
                FundingStreamName = "LegalEntitlementEnglishandMaths",
                ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1),
                RollOverStatus = "To Extend",
                ExclusionReason = "n/a"
            }
        }
            };

            var fakeContext = new FundingExtensionCandidateValidationContext(
                new HashSet<CandidateKey>(),
                new HashSet<CandidateKey>(),
                new HashSet<CandidateKey>());

            _rolloverRepository
                .Setup(r => r.GetFundingExtensionValidationContextAsync(
                    It.IsAny<HashSet<CandidateKey>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeContext);

            var validationResult = new FundingExtensionValidationResult
            {
                IsValid = true,
                TotalCandidates = 1,
                FailedCandidateCount = 0,
                FailureSummary = []
            };

            _validator
                .Setup(v => v.Validate(
                    command.FundingExtensionCandidates,
                    fakeContext,
                    It.IsAny<CancellationToken>()))
                .Returns(validationResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);

            Assert.NotNull(result.Value);
            Assert.True(result.Value.IsValid);
            Assert.Equal(1, result.Value.TotalCandidates);
            Assert.Equal(0, result.Value.FailedCandidateCount);
        }

        [Fact]
        public async Task Handle_InvalidValidation_BuildsCsvAndReturnsFile()
        {
            // Arrange
            var command = new ValidateFundingExtensionCandidatesCommand
            {
                FundingExtensionCandidates = new List<FundingExtensionCandidate>
                {
                    new()
                    {
                        RowNumber = 1,
                        Qan = "60110314",
                        FundingStreamName = "LegalEntitlementEnglishandMaths",
                        ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1),
                        RollOverStatus = "To Extend"
                    }
                }
            };

            var fakeContext = new FundingExtensionCandidateValidationContext(
                new HashSet<CandidateKey>(),
                new HashSet<CandidateKey>(),
                new HashSet<CandidateKey>());

            _rolloverRepository
                .Setup(r => r.GetFundingExtensionValidationContextAsync(
                    It.IsAny<HashSet<CandidateKey>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeContext);

            var validationResult = new FundingExtensionValidationResult
            {
                IsValid = false,
                TotalCandidates = 1,
                FailedCandidateCount = 1,
                FailureSummary = new(),
                Candidates =
                [
                    new CandidateValidationResult
                    {
                        CandidateDetails = command.FundingExtensionCandidates[0],
                        Errors = [ new ValidationFailure { Field = "QAN", Message ="Error message here"}]

                    }
                ]
            };

            var csvBytes = new byte[] { 1, 2, 3 };

            _validator
                .Setup(v => v.Validate(
                    command.FundingExtensionCandidates,
                    fakeContext,
                    It.IsAny<CancellationToken>()))
                .Returns(validationResult);

            _csvBuilder
                .Setup(x => x.BuildWithValidationErrors(
                    It.IsAny<List<FundingExtensionCandidateDto>>(),
                    validationResult.Candidates))
                .Returns(csvBytes);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);

            Assert.NotNull(result.Value);
            Assert.False(result.Value.IsValid);
            Assert.Equal(1, result.Value.FailedCandidateCount);
            Assert.Equal(csvBytes, result.Value.ValidatedCandidateFile);

            _csvBuilder.Verify(x =>
                x.BuildWithValidationErrors(
                    It.IsAny<List<FundingExtensionCandidateDto>>(),
                    validationResult.Candidates),
                Times.Once);
        }

        [Fact]
        public async Task Handle_GenericException_ReturnsErrorMessage()
        {
            // Arrange
            var command = new ValidateFundingExtensionCandidatesCommand
            {
                FundingExtensionCandidates = new List<FundingExtensionCandidate>()
            };

            _rolloverRepository
                .Setup(r => r.GetFundingExtensionValidationContextAsync(
                    It.IsAny<HashSet<CandidateKey>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Exception happened"));

            // Act
            var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Exception happened", result.ErrorMessage);
            Assert.IsType<Exception>(result.InnerException);
        }

        [Fact]
        public async Task Handle_NoWorkflowRunsExist_ReturnsFailure()
        {
            // Arrange
            var command = new ValidateFundingExtensionCandidatesCommand
            {
                FundingExtensionCandidates = new List<FundingExtensionCandidate>
                {
                    new()
                    {
                        RowNumber = 1,
                        Qan = "60110314",
                        FundingStreamName = "LegalEntitlementEnglishandMaths",
                        ProposedFundingApprovalEndDate = DateTime.UtcNow.AddYears(1),
                        RollOverStatus = "To Extend"
                    }
                }
            };

            _rolloverRepository
                .Setup(r => r.GetFundingExtensionValidationContextAsync(
                    It.IsAny<HashSet<CandidateKey>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("No workflow runs exist"));

            // Act
            var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("No workflow runs exist", result.ErrorMessage);
        }
    }
}
