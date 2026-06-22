using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Rollover;
using SFA.DAS.AODP.Data.Entities.Rollover;
using SFA.DAS.AODP.Data.Repositories.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Rollover
{
    public class GetRolloverWorkflowCandidatesQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IRolloverRepository> _repositoryMock;
        private readonly GetRolloverWorkflowCandidatesQueryHandler _handler;

        public GetRolloverWorkflowCandidatesQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _repositoryMock = _fixture.Freeze<Mock<IRolloverRepository>>();

            _handler = _fixture.Create<GetRolloverWorkflowCandidatesQueryHandler>();
        }


        [Fact]
        public async Task Handle_ReturnsSuccess_WhenRepositoryReturnsData()
        {
            // Arrange
            var workflowRunId = Guid.NewGuid();

            var candidates = _fixture.Build<RolloverWorkflowCandidate>()
                .With(x => x.RolloverWorkflowRunId, workflowRunId)
                .CreateMany(3)
                .ToList();

            var workflowRun = _fixture.Build<RolloverWorkflowRun>()
                .With(x => x.Id, workflowRunId)
                .Create();

            _repositoryMock
                .Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            _repositoryMock
                .Setup(r => r.GeRolloverWorkflowRunByIdAsync(workflowRunId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(workflowRun);

            var query = new GetRolloverWorkflowCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(workflowRunId, result.Value.WorkflowRunId);
            Assert.Equal(3, result.Value.RolloverWorkflowCandidates.Count());
            Assert.Equal(workflowRun.OperationalEndDateEligibilityThreshold,
                         result.Value.FundingEndDateEligibilityThreshold);
        }

        // ------------------------------------------------------------
        // FAILURE — WORKFLOW RUN NOT FOUND
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_ReturnsFailure_WhenWorkflowRunNotFound()
        {
            // Arrange
            var workflowRunId = Guid.NewGuid();

            var candidates = _fixture.Build<RolloverWorkflowCandidate>()
                .With(x => x.RolloverWorkflowRunId, workflowRunId)
                .CreateMany(2)
                .ToList();

            _repositoryMock
                .Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            _repositoryMock
                .Setup(r => r.GeRolloverWorkflowRunByIdAsync(workflowRunId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RolloverWorkflowRun?)null);

            var query = new GetRolloverWorkflowCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal($"Workflow run {workflowRunId} was not found.", result.ErrorMessage);
        }

        // ------------------------------------------------------------
        // FAILURE — NO CANDIDATES RETURNED
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_ReturnsFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<RolloverWorkflowCandidate>?)null);

            var query = new GetRolloverWorkflowCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No rollover workflow candidates found.", result.ErrorMessage);
        }

        // ------------------------------------------------------------
        // FAILURE — EXCEPTION THROWN
        // ------------------------------------------------------------
        [Fact]
        public async Task Handle_ReturnsFailure_WhenExceptionThrown()
        {
            // Arrange
            var exception = new InvalidOperationException("DB exploded");

            _repositoryMock
                .Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var query = new GetRolloverWorkflowCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("DB exploded", result.ErrorMessage);
            Assert.Same(exception, result.InnerException);
        }
    }
}
