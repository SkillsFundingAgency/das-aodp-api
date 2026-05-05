
using SFA.DAS.AODP.Application.Commands.Files;
using SFA.DAS.AODP.Data.Entities.Files;
using SFA.DAS.AODP.Data.Repositories.Files;
using SFA.DAS.AODP.Models.Files;
using Moq;

namespace SFA.DAS.AODP.Application.Tests.Commands.Files
{
    public class CreateFileMetadataCommandHandlerTests
    {
        private readonly Mock<IFileRecordRepository> _repository = new();
        private readonly CreateFileMetadataCommandHandler _handler;

        public CreateFileMetadataCommandHandlerTests()
        {
            _handler = new CreateFileMetadataCommandHandler(_repository.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_AddsFileRecord_AndReturnsSuccess()
        {
            // Arrange
            var command = new CreateFileMetadataCommand
            {
                FileCategory = FileCategory.QuestionUpload,
                ApplicationId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                QuestionId = Guid.NewGuid(),
                FileName = "test.pdf",
                ContentType = "application/pdf",
                BlobContainer = "files",
                BlobPath = "some/path/test.pdf",
                UploadedBy = "Alice Admin"
            };

            var record = new FileRecord
            {
                FileCategory = FileCategory.QuestionUpload,
                ApplicationId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                QuestionId = Guid.NewGuid(),
                FileName = "test.pdf",
                ContentType = "application/pdf",
                BlobContainer = "files",
                BlobPath = "some/path/test.pdf",
                UploadedByDisplayName = "Mike"
            };

            _repository
                .Setup(r => r.AddAsync(It.IsAny<FileRecord>()))
                .ReturnsAsync(record);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Null(result.ErrorMessage);
                Assert.Null(result.InnerException);

                _repository.Verify(r =>
                    r.AddAsync(It.Is<FileRecord>(f =>
                        f.FileCategory == command.FileCategory &&
                        f.ApplicationId == command.ApplicationId &&
                        f.MessageId == command.MessageId &&
                        f.QuestionId == command.QuestionId &&
                        f.FileName == command.FileName &&
                        f.ContentType == command.ContentType &&
                        f.BlobContainer == command.BlobContainer &&
                        f.BlobPath == command.BlobPath &&
                        f.UploadedByDisplayName == command.UploadedBy &&
                        f.ScanResult == MalwareScanStatus.NotScanned &&
                        f.UploadedAt <= DateTime.UtcNow
                    )),
                    Times.Once);
            });
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var command = new CreateFileMetadataCommand
            {
                FileCategory = FileCategory.MessageAttachment,
                ApplicationId = Guid.NewGuid(),
                FileName = "test.pdf"
            };

            _repository
                .Setup(r => r.AddAsync(It.IsAny<FileRecord>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.NotNull(result.ErrorMessage);
                Assert.Contains("DB error", result.ErrorMessage);
                Assert.NotNull(result.InnerException);

                _repository.Verify(
                    r => r.AddAsync(It.IsAny<FileRecord>()),
                    Times.Once);
            });
        }
    }
}
