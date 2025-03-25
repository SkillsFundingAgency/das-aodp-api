using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersOutcomeCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationFundingFeedbackRepository> _qualificationFundingFeedbackRepositoryMock;
        private readonly SaveQualificationsFundingOffersOutcomeCommandHandler _handler;

        public SaveQualificationsFundingOffersOutcomeCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _qualificationFundingFeedbackRepositoryMock = _fixture.Freeze<Mock<IQualificationFundingFeedbackRepository>>();
            _handler = new SaveQualificationsFundingOffersOutcomeCommandHandler(
                _qualificationFundingFeedbackRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenFeedbackIsCreated()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersOutcomeCommand>();
            _qualificationFundingFeedbackRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync((QualificationFundingFeedbacks)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationFundingFeedbackRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<QualificationFundingFeedbacks>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenFeedbackIsUpdated()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersOutcomeCommand>();
            var existingFeedback = _fixture.Create<QualificationFundingFeedbacks>();

            _qualificationFundingFeedbackRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFeedback);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationFundingFeedbackRepositoryMock.Verify(repo => repo.UpdateAsync(existingFeedback), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersOutcomeCommand>();
            var exception = new Exception("Test exception");

            _qualificationFundingFeedbackRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ThrowsAsync(exception);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(exception.Message, result.ErrorMessage);
            Assert.Equal(exception, result.InnerException);
        }
    }
}


