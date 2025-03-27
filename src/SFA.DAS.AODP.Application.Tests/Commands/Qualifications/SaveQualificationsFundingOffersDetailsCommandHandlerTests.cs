using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersDetailsCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationFundingsRepository> _qualificationFundingsRepositoryMock;
        private readonly Mock<IQualificationDiscussionHistoryRepository> _qualificationDiscussionHistoryRepositoryMock;
        private readonly SaveQualificationsFundingOffersDetailsCommandHandler _handler;

        public SaveQualificationsFundingOffersDetailsCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());

            _qualificationFundingsRepositoryMock = _fixture.Freeze<Mock<IQualificationFundingsRepository>>();
            _qualificationDiscussionHistoryRepositoryMock = _fixture.Freeze<Mock<IQualificationDiscussionHistoryRepository>>();

            _handler = new SaveQualificationsFundingOffersDetailsCommandHandler(
                _qualificationFundingsRepositoryMock.Object, _qualificationDiscussionHistoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenDataIsProcessed()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersDetailsCommand>();
            var existingFundings = _fixture.CreateMany<QualificationFundings>(command.Details.Count).ToList();

            foreach (var funding in existingFundings)
            {
                funding.FundingOfferId = command.Details[existingFundings.IndexOf(funding)].FundingOfferId;
            }

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationFundingsRepositoryMock.Verify(repo => repo.UpdateAsync(existingFundings), Times.Once);
            _qualificationDiscussionHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<QualificationDiscussionHistory>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenFundingOfferNotFound()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersDetailsCommand>();
            var existingFundings = _fixture.CreateMany<QualificationFundings>(command.Details.Count - 1).ToList();

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.IsType<RecordNotFoundException>(result.InnerException);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersDetailsCommand>();
            var exception = new Exception("Test exception");

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ThrowsAsync(exception);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(exception.Message, result.ErrorMessage);
            Assert.Equal(exception, result.InnerException);
        }

        [Fact]
        public async Task Handle_CreatesDiscussionHistoryNotes_WhenDetailsAreProvided()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersDetailsCommand>();
            var existingFundings = _fixture.CreateMany<QualificationFundings>(command.Details.Count).ToList();

            foreach (var funding in existingFundings)
            {
                funding.FundingOfferId = command.Details[existingFundings.IndexOf(funding)].FundingOfferId;
            }

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationDiscussionHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<QualificationDiscussionHistory>(qdh =>
                qdh.Notes.Contains("The following offers details have been selected"))), Times.Once);
        }

        [Fact]
        public async Task Handle_CreatesDiscussionHistoryNotes_WhenNoDetailsAreProvided()
        {
            // Arrange
            var command = _fixture.Build<SaveQualificationsFundingOffersDetailsCommand>()
                .With(x => x.Details, new List<SaveQualificationsFundingOffersDetailsCommand.OfferFundingDetails>())
                .Create();
            var existingFundings = _fixture.CreateMany<QualificationFundings>(2).ToList();

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationDiscussionHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<QualificationDiscussionHistory>(qdh =>
                qdh.Notes.Contains("No funding offers have been selected"))), Times.Once);
        }
    }
}
