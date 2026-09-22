using Moq;
using SFA.DAS.AODP.Application.Queries.QaaQualification;
using SFA.DAS.AODP.Data.Entities.QaaQualification;
using SFA.DAS.AODP.Data.Repositories.QaaQualification;

namespace SFA.DAS.AODP.Application.UnitTests.Queries.QaaQualification
{
    public class GetQaaDownloadSummaryQueryHandlerTests
    {
        private readonly Mock<IQaaQualificationRepository> _qaaQualificationRepositoryMock;
        private readonly Mock<IQaaQualificationDownloadLogRepository> _downloadLogRepositoryMock;
        private readonly GetQaaDownloadSummaryQueryHandler _handler;

        public GetQaaDownloadSummaryQueryHandlerTests()
        {
            _qaaQualificationRepositoryMock = new Mock<IQaaQualificationRepository>();
            _downloadLogRepositoryMock = new Mock<IQaaQualificationDownloadLogRepository>();
            _handler = new GetQaaDownloadSummaryQueryHandler(
                _qaaQualificationRepositoryMock.Object,
                _downloadLogRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Success_MapsCountsAndHistory()
        {
            // Arrange
            var lastImported = new DateTime(2026, 2, 17);
            _qaaQualificationRepositoryMock
                .Setup(r => r.GetSummaryCountsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QaaQualificationSummaryCounts
                {
                    DataLastImportedDate = lastImported,
                    NewCount = 1,
                    ExtendedCount = 2,
                    DiscontinuedCount = 3
                });

            var olderDownload = new QaaQualificationDownloadLog { UserDisplayName = "user1", DownloadDate = new DateTime(2026, 3, 1) };
            var newerDownload = new QaaQualificationDownloadLog { UserDisplayName = "user2", DownloadDate = new DateTime(2026, 3, 5) };

            _downloadLogRepositoryMock
                .Setup(r => r.ListAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { olderDownload, newerDownload });

            // Act
            var result = await _handler.Handle(new GetQaaDownloadSummaryQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(lastImported, result.Value!.DataLastImportedDate);
            Assert.Equal(new DateTime(2026, 3, 5), result.Value.MostRecentDownloadDate);
            Assert.Equal(1, result.Value.NewQualificationsCount);
            Assert.Equal(2, result.Value.ExtendedQualificationsCount);
            Assert.Equal(3, result.Value.DiscontinuedQualificationsCount);
            Assert.Equal(2, result.Value.DownloadHistory.Count);
        }

        [Fact]
        public async Task Handle_NoDownloadHistory_ReturnsNullMostRecentDownloadDate()
        {
            // Arrange
            _qaaQualificationRepositoryMock
                .Setup(r => r.GetSummaryCountsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QaaQualificationSummaryCounts());

            _downloadLogRepositoryMock
                .Setup(r => r.ListAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<QaaQualificationDownloadLog>());

            // Act
            var result = await _handler.Handle(new GetQaaDownloadSummaryQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Value!.MostRecentDownloadDate);
            Assert.Empty(result.Value.DownloadHistory);
        }

        [Fact]
        public async Task Handle_RepositoryThrows_ReturnsFailure()
        {
            // Arrange
            _qaaQualificationRepositoryMock
                .Setup(r => r.GetSummaryCountsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("boom"));

            // Act
            var result = await _handler.Handle(new GetQaaDownloadSummaryQuery(), CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("boom", result.ErrorMessage);
            Assert.IsType<InvalidOperationException>(result.InnerException);
        }
    }
}
