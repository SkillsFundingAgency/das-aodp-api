using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification
{
    public class GetQualificationOutputFileLogQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationOutputFileLogRepository> _repo;
        private readonly GetQualificationOutputFileLogQueryHandler _handler;

        public GetQualificationOutputFileLogQueryHandlerTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            _repo = _fixture.Freeze<Mock<IQualificationOutputFileLogRepository>>();
            _handler = _fixture.Create<GetQualificationOutputFileLogQueryHandler>();
        }

        [Fact]
        public async Task Then_Repository_Is_Called_And_Returns_Success_With_Logs()
        {
            // Arrange
            var logs = new List<QualificationOutputFileLog>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    UserDisplayName = "Alice",
                    Timestamp = DateTime.UtcNow.AddMinutes(-5),
                    ApprovedFileName = "2025-10-17-AOdPApprovedOutputFile.csv",
                    ArchivedFileName = "2025-10-17-AOdPArchivedOutputFile.csv"
                }
            };

            _repo.Setup(r => r.ListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(logs);

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileLogQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(r => r.ListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.NotNull(result.Value!.OutputFileLogs);
            Assert.Single(result.Value.OutputFileLogs);
        }

        [Fact]
        public async Task Then_Repository_Is_Called_And_Returns_Success_With_Empty_List()
        {
            // Arrange
            _repo.Setup(r => r.ListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<QualificationOutputFileLog>());

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileLogQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(r => r.ListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.NotNull(result.Value!.OutputFileLogs);
            Assert.Empty(result.Value.OutputFileLogs);
        }

        [Fact]
        public async Task Then_Null_From_Repository_Returns_Failure()
        {
            // Arrange
            _repo.Setup(r => r.ListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((List<QualificationOutputFileLog>?)null);

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileLogQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(r => r.ListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal("No logs found.", result.ErrorMessage);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Then_Exception_Is_Handled_And_Failure_Returned()
        {
            // Arrange
            _repo.Setup(r => r.ListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("boom"));

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileLogQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(r => r.ListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal("boom", result.ErrorMessage);
            Assert.Null(result.Value);
        }
    }
}
