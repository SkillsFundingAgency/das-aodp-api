using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Moq;
using SFA.DAS.AODP.Application.Commands.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;


namespace SFA.DAS.AODP.Application.UnitTests.Commands.Qualifications
{
    public class CreateQualificationDiscussionHistoryCommandHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationDiscussionHistoryRepository> _discussionHistoryRepositoryMock;
        private readonly Mock<IQualificationFundingsRepository> _qualificationFundingsRepositoryMock;
        private readonly Mock<IQualificationsRepository> _qualificationsRepositoryMock;
        private readonly CreateQualificationDiscussionHistoryCommandHandler _handler;

        public CreateQualificationDiscussionHistoryCommandHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());

            _discussionHistoryRepositoryMock = _fixture.Freeze<Mock<IQualificationDiscussionHistoryRepository>>();
            _qualificationFundingsRepositoryMock = _fixture.Freeze<Mock<IQualificationFundingsRepository>>();
            _qualificationsRepositoryMock = _fixture.Freeze<Mock<IQualificationsRepository>>();
            _handler = new CreateQualificationDiscussionHistoryCommandHandler(
                _discussionHistoryRepositoryMock.Object,
                _qualificationFundingsRepositoryMock.Object,
                _qualificationsRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenDataIsFound()
        {
            // Arrange
            var command = _fixture.Create<CreateQualificationDiscussionHistoryCommand>();
            var fundings = _fixture.CreateMany<QualificationFundings>(2).ToList();
            var qualification = _fixture.Create<Qualification>();

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(fundings);
            _qualificationsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationReference))
                .ReturnsAsync(qualification);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _discussionHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<QualificationDiscussionHistory>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenNoFundingsFound()
        {
            // Arrange
            var command = _fixture.Create<CreateQualificationDiscussionHistoryCommand>();
            var qualification = _fixture.Create<Qualification>();

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(new List<QualificationFundings>());
            _qualificationsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationReference))
                .ReturnsAsync(qualification);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _discussionHistoryRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<QualificationDiscussionHistory>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var command = _fixture.Create<CreateQualificationDiscussionHistoryCommand>();
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
        public async Task Handle_ReturnsFailureResponse_WhenQualificationNotFound()
        {
            // Arrange
            var command = _fixture.Create<CreateQualificationDiscussionHistoryCommand>();

            _qualificationFundingsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationVersionId))
                .ReturnsAsync(new List<QualificationFundings>());
            _qualificationsRepositoryMock.Setup(repo => repo.GetByIdAsync(command.QualificationReference))
                .ReturnsAsync((Qualification)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Qualification not found", ((Data.Exceptions.RecordWithNameNotFoundException)result.InnerException).Name);
         }
    }

    public class DateOnlySpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type == typeof(DateOnly))
            {
                return DateOnly.FromDateTime(DateTime.Now);
            }

            return new NoSpecimen();
        }
    }
}



