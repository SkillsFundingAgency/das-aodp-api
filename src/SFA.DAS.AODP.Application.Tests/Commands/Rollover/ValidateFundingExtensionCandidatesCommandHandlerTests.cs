using Moq;
using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Services;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Tests.Commands.Rollover
{
    public class ValidateFundingExtensionCandidatesCommandHandlerTests
    {
        private readonly Mock<IRolloverRepository> _rolloverRepository = new();
        private readonly Mock<IRolloverFundingExtensionValidator> _validator = new();
        private readonly ValidateFundingExtensionCandidatesCommandHandler _handler;

        public ValidateFundingExtensionCandidatesCommandHandlerTests()
        {
            _handler = new ValidateFundingExtensionCandidatesCommandHandler(
                _rolloverRepository.Object,
                _validator.Object);
        }

        [Fact]
        public async Task Handle_Success_ReturnsValidatorResult()
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
                        ProposedFundingEndDate = DateTime.UtcNow.AddYears(1),
                        RolloverStatus = "To Extend",
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

            var expectedResponse = new ValidateFundingExtensionCandidatesCommandResponse
            {
                TotalCandidates = 1,
                FailedCandidateCount = 0,
                IsValid = true
            };

            _validator
                .Setup(v => v.Validate(
                    command.FundingExtensionCandidates,
                    fakeContext,
                    It.IsAny<CancellationToken>()))
                .Returns(expectedResponse);

            // Act
            var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            Assert.True(result.Success);
            Assert.Same(expectedResponse, result.Value);
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
                        ProposedFundingEndDate = DateTime.UtcNow.AddYears(1),
                        RolloverStatus = "To Extend"
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
