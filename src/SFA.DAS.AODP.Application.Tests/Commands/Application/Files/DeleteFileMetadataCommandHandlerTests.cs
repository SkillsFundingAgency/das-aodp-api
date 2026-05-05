using Moq;
using SFA.DAS.AODP.Application.Commands.Files;
using SFA.DAS.AODP.Data.Entities.Files;
using SFA.DAS.AODP.Data.Repositories.Files;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using Xunit;

namespace SFA.DAS.AODP.Application.Tests.Commands.Files
{
    public class DeleteFileCommandHandlerTests
    {
        private readonly Mock<IFileRecordRepository> _repository = new();
        private readonly Mock<IBlobStorageService> _blobStorageService = new();
        private readonly DeleteFileCommandHandler _handler;

        public DeleteFileCommandHandlerTests()
        {
            _handler = new DeleteFileCommandHandler(
                _repository.Object,
                _blobStorageService.Object);
        }

        [Fact]
        public async Task Handle_FileExists_DeletesBlobAndRecord_ReturnsSuccess()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            var fileMetadata = new FileRecord
            {
                Id = fileId,
                BlobContainer = "container",
                BlobPath = "path/file.pdf"
            };

            var command = new DeleteFileCommand { FileId = fileId };

            _repository
                .Setup(r => r.GetByIdAsync(fileId))
                .ReturnsAsync(fileMetadata);

            _blobStorageService
                .Setup(b => b.DeleteAsync(fileMetadata.BlobContainer, fileMetadata.BlobPath))
                .Returns(Task.CompletedTask);

            _repository
                .Setup(r => r.DeleteAsync(fileId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.True(result.Success);
                Assert.Null(result.ErrorMessage);
                Assert.Null(result.InnerException);

                _blobStorageService.Verify(b =>
                        b.DeleteAsync(fileMetadata.BlobContainer, fileMetadata.BlobPath),
                    Times.Once);

                _repository.Verify(r => r.DeleteAsync(fileId), Times.Once);
            });
        }

        [Fact]
        public async Task Handle_FileDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            var command = new DeleteFileCommand { FileId = fileId };

            _repository
                .Setup(r => r.GetByIdAsync(fileId))
                .ReturnsAsync((FileRecord?)null);

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.Equal("File not found", result.ErrorMessage);

                _blobStorageService.Verify(
                    b => b.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()),
                    Times.Never);

                _repository.Verify(
                    r => r.DeleteAsync(It.IsAny<Guid>()),
                    Times.Never);
            });
        }

        [Fact]
        public async Task Handle_BlobDeleteThrowsException_ReturnsFailure()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            var fileMetadata = new FileRecord
            {
                Id = fileId,
                BlobContainer = "container",
                BlobPath = "path/file.pdf"
            };

            var command = new DeleteFileCommand { FileId = fileId };

            _repository
                .Setup(r => r.GetByIdAsync(fileId))
                .ReturnsAsync(fileMetadata);

            _blobStorageService
                .Setup(b => b.DeleteAsync(fileMetadata.BlobContainer, fileMetadata.BlobPath))
                .ThrowsAsync(new Exception("Blob delete failed"));

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.Contains("Blob delete failed", result.ErrorMessage);
                Assert.NotNull(result.InnerException);

                _repository.Verify(
                    r => r.DeleteAsync(It.IsAny<Guid>()),
                    Times.Never);
            });
        }

        [Fact]
        public async Task Handle_RepositoryDeleteThrowsException_ReturnsFailure()
        {
            // Arrange
            var fileId = Guid.NewGuid();

            var fileMetadata = new FileRecord
            {
                Id = fileId,
                BlobContainer = "container",
                BlobPath = "path/file.pdf"
            };

            var command = new DeleteFileCommand { FileId = fileId };

            _repository
                .Setup(r => r.GetByIdAsync(fileId))
                .ReturnsAsync(fileMetadata);

            _blobStorageService
                .Setup(b => b.DeleteAsync(fileMetadata.BlobContainer, fileMetadata.BlobPath))
                .Returns(Task.CompletedTask);

            _repository
                .Setup(r => r.DeleteAsync(fileId))
                .ThrowsAsync(new Exception("DB delete failed"));

            // Act
            var result = await _handler.Handle(command, default);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.False(result.Success);
                Assert.Contains("DB delete failed", result.ErrorMessage);
                Assert.NotNull(result.InnerException);
            });
        }
    }
}