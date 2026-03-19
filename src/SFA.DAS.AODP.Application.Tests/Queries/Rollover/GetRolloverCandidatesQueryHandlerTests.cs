using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Rollover
{
    public class GetRolloverCandidatesQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IRolloverRepository> _repositoryMock;
        private readonly GetRolloverCandidatesQueryHandler _handler;

        public GetRolloverCandidatesQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IRolloverRepository>>();
            _handler = _fixture.Create<GetRolloverCandidatesQueryHandler>();
        }

        [Fact]
        public async Task Handle_ReturnsSuccess_WhenRepositoryReturnsData()
        {
            // Arrange
            var candidates = _fixture.CreateMany<RolloverCandidate>(3).ToList();

            _repositoryMock
                .Setup(r => r.GetRolloverCandidatesAsync(default))
                .ReturnsAsync(candidates);

            var query = new GetRolloverCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(candidates, result.Value.RolloverCandidates);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ReturnsFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetRolloverCandidatesAsync(default))
                .ReturnsAsync((List<RolloverCandidate>)null!);

            var query = new GetRolloverCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No rollover candidates found.", result.ErrorMessage);
        }


        [Fact]
        public async Task Handle_ReturnsFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var exception = new InvalidOperationException("DB exploded");

            _repositoryMock
                .Setup(r => r.GetRolloverCandidatesAsync(default))
                .ThrowsAsync(exception);

            var query = new GetRolloverCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("DB exploded", result.ErrorMessage);
            Assert.Same(exception, result.InnerException);
        }
    }
}