using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualification;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification
{
    public class GetQualificationVersionsForQualificationByReferenceQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationsRepository> _qualificationsRepositoryMock;
        private readonly GetQualificationVersionsForQualificationByReferenceQueryHandler _handler;

        public GetQualificationVersionsForQualificationByReferenceQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _qualificationsRepositoryMock = _fixture.Freeze<Mock<IQualificationsRepository>>();
            _handler = new GetQualificationVersionsForQualificationByReferenceQueryHandler(
                _qualificationsRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsSuccessResponse_WhenDataIsFound()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationVersionsForQualificationByReferenceQuery>();
            var qualification = _fixture.Create<Data.Entities.Qualification.Qualification>();

            _qualificationsRepositoryMock.Setup(repo => repo.GetByIdAsync(query.QualificationReference))
                .ReturnsAsync(qualification);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(qualification.Id, result.Value.Id);
            Assert.Equal(qualification.Qan, result.Value.QualificationReference);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenExceptionIsThrown()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationVersionsForQualificationByReferenceQuery>();
            var exception = new Exception("Test exception");

            _qualificationsRepositoryMock.Setup(repo => repo.GetByIdAsync(query.QualificationReference))
                .ThrowsAsync(exception);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(exception.Message, result.ErrorMessage);
            Assert.Equal(exception, result.InnerException);
        }

        [Fact]
        public async Task Handle_ReturnsFailureResponse_WhenQualificationNotFound()
        {
            // Arrange
            var query = _fixture.Create<GetQualificationVersionsForQualificationByReferenceQuery>();

            _qualificationsRepositoryMock.Setup(repo => repo.GetByIdAsync(query.QualificationReference))
                .ReturnsAsync((Data.Entities.Qualification.Qualification)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Qualification not found", result.ErrorMessage);
        }
    }
}
