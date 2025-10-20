using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using Xunit;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.Qualification
{
    public class GetQualificationOutputFileLogQueryHandlerTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IQualificationOutputFileLogRepository> _repo;
        private readonly GetQualificationOutputFileLogQueryHandler _handler;

        private const string ApprovedSuffix = "-AOdPApprovedOutputFile.csv";
        private const string ArchivedSuffix = "-AOdPArchivedOutputFile.csv";
        private const string ErrorNoLogs = "No logs found.";
        private const string ExceptionMessage = "Problem found";

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
            var datePrefix = DateTime.Now.ToString("yy-MM-dd");
            var logs = new List<QualificationOutputFileLog>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    UserDisplayName = "Alice",
                    Timestamp = DateTime.UtcNow.AddMinutes(-5),
                    ApprovedFileName = $"{datePrefix}{ApprovedSuffix}",
                    ArchivedFileName = $"{datePrefix}{ArchivedSuffix}"
                }
            };

            _repo.Setup(r => r.ListAsync(null, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(logs);

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileLogQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(r => r.ListAsync(null, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Multiple(() =>
            {
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.NotNull(result.Value!.OutputFileLogs);
            Assert.Single(result.Value.OutputFileLogs);
            });
        }

        [Fact]
        public async Task Then_Repository_Is_Called_And_Returns_Success_With_Empty_List()
        {
            // Arrange
            _repo.Setup(r => r.ListAsync(null, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<QualificationOutputFileLog>());

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileLogQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(r => r.ListAsync(null, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Multiple(() =>
            {
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.NotNull(result.Value!.OutputFileLogs);
            Assert.Empty(result.Value.OutputFileLogs);
            });
        }

        [Fact]
        public async Task Then_Null_From_Repository_Returns_Failure()
        {
            // Arrange
            _repo.Setup(r => r.ListAsync(null, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((List<QualificationOutputFileLog>?)null!);

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileLogQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(r => r.ListAsync(null, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Multiple(() =>
            {
            Assert.False(result.Success);
                Assert.Equal(ErrorNoLogs, result.ErrorMessage);
                Assert.NotNull(result.Value);
            });
        }

        [Fact]
        public async Task Then_Exception_Is_Handled_And_Failure_Returned()
        {
            // Arrange
            _repo.Setup(r => r.ListAsync(null, It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception(ExceptionMessage));

            // Act
            var result = await _handler.Handle(new GetQualificationOutputFileLogQuery(), CancellationToken.None);

            // Assert
            _repo.Verify(r => r.ListAsync(null, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Multiple(() =>
            {
            Assert.False(result.Success);
                Assert.Equal(ExceptionMessage, result.ErrorMessage);
                Assert.NotNull(result.Value);
            });
        }
    }
}
