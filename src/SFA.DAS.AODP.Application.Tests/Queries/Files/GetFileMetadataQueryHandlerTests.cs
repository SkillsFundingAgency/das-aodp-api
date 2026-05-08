using Moq;
using SFA.DAS.AODP.Application.Queries.Files;
using SFA.DAS.AODP.Data.Entities.Files;
using SFA.DAS.AODP.Data.Repositories.Files;
using SFA.DAS.AODP.Models.Files;
using Xunit;

namespace SFA.DAS.AODP.Application.Tests.Queries.Files
{
    public class GetFileMetadataQueryHandlerTests
    {
        private readonly Mock<IFileRecordRepository> _repository = new();
        private readonly GetFileMetadataQueryHandler _handler;

        public GetFileMetadataQueryHandlerTests()
        {
            _handler = new GetFileMetadataQueryHandler(_repository.Object);
        }

        [Fact]
        public async Task Handle_WithFileId_ReturnsSingleFile_WhenFound()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            var record = new FileRecord
            {
                Id = fileId,
                FileName = "file.pdf",
                BlobContainer = "container",
                BlobPath = "path/file.pdf",
                ContentType = "application/pdf",
                ScanResult = MalwareScanStatus.Clean
            };

            var query = new GetFileMetadataQuery
            {
                FileId = fileId
            };

            _repository
                .Setup(r => r.GetByIdAsync(fileId))
                .ReturnsAsync(record);

            // Act
            var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Single(result.Value.Files);

                var file = result.Value.Files.Single();
                Assert.Equal(fileId, file.FileId);
                Assert.Equal("file.pdf", file.FileName);
                Assert.True(file.IsDownloadable);
            });
        }

        [Fact]
        public async Task Handle_WithFileId_ReturnsEmptyList_WhenNotFound()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            var query = new GetFileMetadataQuery
            {
                FileId = fileId
            };

            _repository
                .Setup(r => r.GetByIdAsync(fileId))
                .ReturnsAsync((FileRecord?)null);

            // Act
            var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Empty(result.Value.Files);
            });
        }

        [Fact]
        public async Task Handle_WithoutFileId_ReturnsFilesFromSearch()
        {
            // Arrange
            var records = new List<FileRecord>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FileName = "clean.pdf",
                    BlobContainer = "container",
                    BlobPath = "path/clean.pdf",
                    ContentType = "application/pdf",
                    ScanResult = MalwareScanStatus.Clean
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FileName = "infected.pdf",
                    BlobContainer = "container",
                    BlobPath = "path/infected.pdf",
                    ContentType = "application/pdf",
                    ScanResult = MalwareScanStatus.Malicious
                }
            };

            var query = new GetFileMetadataQuery
            {
                FileCategory = FileCategory.MessageAttachment,
                ApplicationId = Guid.NewGuid()
            };

            _repository
                .Setup(r => r.GetFilesAsync(
                    query.FileCategory,
                    query.ApplicationId,
                    query.MessageId,
                    query.QuestionId))
                .ReturnsAsync(records);

            // Act
            var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(2, result.Value.Files.Count);

                var cleanFile = result.Value.Files.First(f => f.FileName == "clean.pdf");
                var infectedFile = result.Value.Files.First(f => f.FileName == "infected.pdf");

                Assert.True(cleanFile.IsDownloadable);
                Assert.False(infectedFile.IsDownloadable);
            });
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var query = new GetFileMetadataQuery
            {
                FileCategory = FileCategory.QuestionUpload
            };

            _repository
                .Setup(r => r.GetFilesAsync(
                    It.IsAny<FileCategory>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<Guid?>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.NotNull(result.Value);
                Assert.NotNull(result.ErrorMessage);
                Assert.Empty(result.Value.Files);
                Assert.Contains("Database error", result.ErrorMessage);
                Assert.NotNull(result.InnerException);
            });
        }
    }
}
