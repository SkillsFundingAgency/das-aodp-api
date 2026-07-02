using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.FundingOffer;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using SFA.DAS.AODP.Shared.UnitTests.Helpers;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationFundingsRepository> _qualificationFundingsRepositoryMock;
        private readonly Mock<IQualificationDiscussionHistoryRepository> _qualificationDiscussionHistoryRepositoryMock;
        private readonly Mock<IFundingOfferRepository> _fundingOfferRepositoryMock;
        private readonly Mock<IQualificationsRepository> _qualificationsRepositoryMock;
        private readonly SaveQualificationsFundingOffersCommandHandler _handler;

        public SaveQualificationsFundingOffersCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());

            _qualificationFundingsRepositoryMock = _fixture.Freeze<Mock<IQualificationFundingsRepository>>();
            _qualificationDiscussionHistoryRepositoryMock = _fixture.Freeze<Mock<IQualificationDiscussionHistoryRepository>>();
            _qualificationsRepositoryMock = _fixture.Freeze<Mock<IQualificationsRepository>>();
            _fundingOfferRepositoryMock = _fixture.Freeze<Mock<IFundingOfferRepository>>();

            _handler = new SaveQualificationsFundingOffersCommandHandler(
                _qualificationFundingsRepositoryMock.Object, _qualificationDiscussionHistoryRepositoryMock.Object, _fundingOfferRepositoryMock.Object, _qualificationsRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenDataIsProcessed()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersCommand>();
            var existingFundings = _fixture.CreateMany<QualificationFundings>(2).ToList();
            var fundingOffers = _fixture.CreateMany<FundingOffer>(2).ToList();

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);
            _fundingOfferRepositoryMock.Setup(repo => repo.GetFundingOffersAsync())
                .ReturnsAsync(fundingOffers);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationFundingsRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<List<QualificationFundings>>()), Times.Once);
            _qualificationFundingsRepositoryMock.Verify(repo => repo.RemoveAsync(It.IsAny<List<QualificationFundings>>()), Times.Once);
            _qualificationDiscussionHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<QualificationDiscussionHistory>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenNoChangesAreNeeded()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersCommand>();
            var existingFundings = _fixture.CreateMany<QualificationFundings>(command.SelectedOfferIds.Count).ToList();

            foreach (var funding in existingFundings)
            {
                funding.FundingOfferId = command.SelectedOfferIds[existingFundings.IndexOf(funding)];
            }

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);
            _fundingOfferRepositoryMock.Setup(repo => repo.GetFundingOffersAsync())
                .ReturnsAsync(new List<FundingOffer>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationFundingsRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<List<QualificationFundings>>()), Times.Never);
            _qualificationFundingsRepositoryMock.Verify(repo => repo.RemoveAsync(It.IsAny<List<QualificationFundings>>()), Times.Never);
            _qualificationDiscussionHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<QualificationDiscussionHistory>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersCommand>();
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
        public async Task Handle_CreatesDiscussionHistoryNotes_WhenOffersAreRemoved()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersCommand>();
            var existingFundings = _fixture.CreateMany<QualificationFundings>(2).ToList();
            var fundingOffers = _fixture.CreateMany<FundingOffer>(2).ToList();

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);
            _fundingOfferRepositoryMock.Setup(repo => repo.GetFundingOffersAsync())
                .ReturnsAsync(fundingOffers);

            command.SelectedOfferIds.Clear(); // No offers selected, so all existing offers should be removed

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationDiscussionHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<QualificationDiscussionHistory>(qdh =>
                qdh.Notes.Contains("The following offers have been removed"))), Times.Once);
        }

        [Fact]
        public async Task Handle_CreatesDiscussionHistoryNotes_WhenOffersAreSelected()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersCommand>();
            var existingFundings = new List<QualificationFundings>(); // No existing offers
            var fundingOffers = _fixture.CreateMany<FundingOffer>(2).ToList();

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);
            _fundingOfferRepositoryMock.Setup(repo => repo.GetFundingOffersAsync())
                .ReturnsAsync(fundingOffers);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationDiscussionHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.Is<QualificationDiscussionHistory>(qdh =>
                qdh.Notes.Contains("The following offers have been selected"))), Times.Once);
        }

        [Fact]
        public async Task Handle_CallsRepositoryToGetOperationalStartDate()
        {
            var command = _fixture.Create<SaveQualificationsFundingOffersCommand>();
            var existingFundings = new List<QualificationFundings>();
            var fundingOffers = _fixture.CreateMany<FundingOffer>(2).ToList();

            _qualificationFundingsRepositoryMock
                .Setup(r => r.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);

            _fundingOfferRepositoryMock
                .Setup(r => r.GetFundingOffersAsync())
                .ReturnsAsync(fundingOffers);

            _qualificationsRepositoryMock
                .Setup(r => r.GetQualificationVersionOperationalStartDateById(command.QualificationVersionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DateOnly(2024, 1, 1));

            await _handler.Handle(command, CancellationToken.None);

            _qualificationsRepositoryMock.Verify(r =>
                r.GetQualificationVersionOperationalStartDateById(command.QualificationVersionId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_SetsStartDateOnNewFundingRecords()
        {
            var command = _fixture.Create<SaveQualificationsFundingOffersCommand>();
            command.SelectedOfferIds = new List<Guid> { Guid.NewGuid() };

            var existingFundings = new List<QualificationFundings>();
            var fundingOffers = _fixture.CreateMany<FundingOffer>(1).ToList();

            var expectedStartDate = new DateOnly(2024, 1, 1);

            _qualificationFundingsRepositoryMock
                .Setup(r => r.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);

            _fundingOfferRepositoryMock
                .Setup(r => r.GetFundingOffersAsync())
                .ReturnsAsync(fundingOffers);

            _qualificationsRepositoryMock
                .Setup(r => r.GetQualificationVersionOperationalStartDateById(command.QualificationVersionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStartDate);

            List<QualificationFundings> created = null!;
            _qualificationFundingsRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<List<QualificationFundings>>()))
                .Callback<List<QualificationFundings>>(c => created = c);

            await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(created);
            Assert.Single(created);
            Assert.Equal(expectedStartDate, created[0].StartDate);
        }
    }
}