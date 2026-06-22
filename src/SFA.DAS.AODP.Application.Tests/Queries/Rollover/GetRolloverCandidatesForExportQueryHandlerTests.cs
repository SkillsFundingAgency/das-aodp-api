using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Rollover;
using SFA.DAS.AODP.Application.Services.Export;
using SFA.DAS.AODP.Application.UnitTests.Commands.Qualifications;
using SFA.DAS.AODP.Data.Repositories.Rollover;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Rollover
{
    public class GetRolloverCandidatesForExportQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IRolloverRepository> _repositoryMock;
        private readonly Mock<IFundingExtensionCandidatesCsvBuilder> _csvBuilderMock;
        private readonly GetRolloverCandidatesForExportQueryHandler _handler;

        public GetRolloverCandidatesForExportQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());
            _repositoryMock = _fixture.Freeze<Mock<IRolloverRepository>>();
            _csvBuilderMock = _fixture.Freeze<Mock<IFundingExtensionCandidatesCsvBuilder>>();

            _handler = _fixture.Create<GetRolloverCandidatesForExportQueryHandler>();
        }

        [Fact]
        public async Task Handle_ReturnsSuccess_WhenRepositoryReturnsData()
        {
            // Arrange
            var workflowRunId = Guid.NewGuid();
            var candidates = _fixture.CreateMany<RolloverCandidateForExport>(3).ToList();
            var csvBytes = new byte[] { 1, 2, 3 };

            _repositoryMock
                .Setup(r => r.GetRolloverWorkflowCandidatesByRunId(workflowRunId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(candidates);

            _csvBuilderMock
                .Setup(b => b.Build(candidates))
                .Returns(csvBytes);

            var query = new GetRolloverCandidatesForExportQuery
            {
                RolloverWorkflowRunId = workflowRunId
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(csvBytes, result.Value.FileContent);
            Assert.Equal($"RolloverCandidates_SystemDraft_{DateOnly.FromDateTime(DateTime.Today).ToFilenameDateFormat()}.csv", result.Value.FileName);
            Assert.Equal("text/csv", result.Value.ContentType);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ReturnsFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var workflowRunId = Guid.NewGuid();
            var exception = new InvalidOperationException("Exception in DB");

            _repositoryMock
                .Setup(r => r.GetRolloverWorkflowCandidatesByRunId(workflowRunId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var query = new GetRolloverCandidatesForExportQuery
            {
                RolloverWorkflowRunId = workflowRunId
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Exception in DB", result.ErrorMessage);
            Assert.Same(exception, result.InnerException);
        }
    }
}

