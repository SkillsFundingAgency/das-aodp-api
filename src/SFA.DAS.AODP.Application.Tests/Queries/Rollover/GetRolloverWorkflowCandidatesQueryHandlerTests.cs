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
            var workflowRun = _fixture.Build<RolloverWorkflowRun>()
                .Create();

            var candidates = CreateCandidates(3, workflowRun.Id);



            _repositoryMock
                .Setup(r => r.GetAllRolloverWorkflowCandidatesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            _repositoryMock
                .Setup(r => r.GeRolloverWorkflowRunByIdAsync(workflowRun.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(workflowRun);

            var query = new GetRolloverWorkflowCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(workflowRun.Id, result.Value.WorkflowRunId);
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

            var candidates = CreateCandidates(2, workflowRunId);

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

        private List<RolloverWorkflowCandidate> CreateCandidates(int count, Guid workflowRunId)
        {
            List<RolloverWorkflowCandidate> candidates = new List<RolloverWorkflowCandidate>();
            DateTime now = DateTime.UtcNow;

            for(int i = 0; i < count; i++)
            {
                candidates.Add(
                    RolloverWorkflowCandidate.Create(
                    workflowRunId,
                    Guid.NewGuid(),          // rolloverCandidateRecordId
                    Guid.NewGuid(),          // qualificationVersionId
                    Guid.NewGuid(),          // fundingOfferId
                    "2024",                  // academicYear
                    1,                       // rolloverRound
                    now,                     // currentFundingEndDate
                    now.AddYears(1),         // proposedFundingEndDate
                    now                      // createdAt
                ));

            }
            return candidates;
        }
    }
}
