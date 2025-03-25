using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Moq;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.AODP.Application.UnitTests.Commands.Qualifications
{
    public class SaveQualificationsFundingOffersCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationFundingsRepository> _qualificationFundingsRepositoryMock;
        private readonly SaveQualificationsFundingOffersCommandHandler _handler;

        public SaveQualificationsFundingOffersCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());

            _qualificationFundingsRepositoryMock = _fixture.Freeze<Mock<IQualificationFundingsRepository>>();
            _handler = new SaveQualificationsFundingOffersCommandHandler(
                _qualificationFundingsRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenDataIsProcessed()
        {
            // Arrange
            var command = _fixture.Create<SaveQualificationsFundingOffersCommand>();
            var existingFundings = _fixture.CreateMany<QualificationFundings>(2).ToList();

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(existingFundings);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationFundingsRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<List<QualificationFundings>>()), Times.Once);
            _qualificationFundingsRepositoryMock.Verify(repo => repo.RemoveAsync(It.IsAny<List<QualificationFundings>>()), Times.Once);
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

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _qualificationFundingsRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<List<QualificationFundings>>()), Times.Never);
            _qualificationFundingsRepositoryMock.Verify(repo => repo.RemoveAsync(It.IsAny<List<QualificationFundings>>()), Times.Never);
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
    }
}


