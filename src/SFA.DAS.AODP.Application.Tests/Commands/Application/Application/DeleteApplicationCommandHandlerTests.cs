using Moq;
using SFA.DAS.AODP.Application.Commands.Application.Application;
using SFA.DAS.AODP.Data.Entities.Files;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.Files;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using Xunit;

namespace SFA.DAS.AODP.Application.Tests.Commands.Application.Application;

public class DeleteApplicationCommandHandlerTests
{
    private readonly Mock<IApplicationRepository> _applicationRepository = new();
    private readonly Mock<IFileRecordRepository> _fileRecordRepository = new();
    private readonly Mock<IBlobStorageService> _blobStorageService = new();
    private readonly DeleteApplicationCommandHandler _handler;

    private static readonly Guid ApplicationId = Guid.NewGuid();
    private const string ExceptionMessage = "Delete failed";

    public DeleteApplicationCommandHandlerTests()
    {
        _handler = new DeleteApplicationCommandHandler(
            _applicationRepository.Object,
            _fileRecordRepository.Object,
            _blobStorageService.Object);
    }

    [Fact]
    public async Task Handle_NotSubmitted_WithFiles_DeletesFilesBlobsAndApplication()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false
        };

        var files = new List<FileRecord>
        {
            new() { Id = Guid.NewGuid(), BlobContainer = "container", BlobPath = "path/one.pdf" },
            new() { Id = Guid.NewGuid(), BlobContainer = "container", BlobPath = "path/two.pdf" }
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _fileRecordRepository
            .Setup(r => r.GetByApplicationIdAsync(ApplicationId))
            .ReturnsAsync(files);

        var command = new DeleteApplicationCommand(ApplicationId);

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);

            foreach (var file in files)
            {
                _blobStorageService.Verify(b =>
                    b.DeleteAsync(file.BlobContainer, file.BlobPath), Times.Once);

                _fileRecordRepository.Verify(r =>
                    r.DeleteAsync(file.Id), Times.Once);
            }

            _applicationRepository.Verify(r =>
                r.DeleteAsync(application), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_NotSubmitted_NoFiles_DeletesApplicationOnly()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _fileRecordRepository
            .Setup(r => r.GetByApplicationIdAsync(ApplicationId))
            .ReturnsAsync(new List<FileRecord>());

        var command = new DeleteApplicationCommand(ApplicationId);

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.True(response.Success);

            _blobStorageService.Verify(b =>
                b.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            _fileRecordRepository.Verify(r =>
                r.DeleteAsync(It.IsAny<Guid>()), Times.Never);

            _applicationRepository.Verify(r =>
                r.DeleteAsync(application), Times.Once);
        });
    }

    [Fact]
    public async Task Handle_Submitted_ReturnsError_AndDoesNotDeleteAnything()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = true
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        var command = new DeleteApplicationCommand(ApplicationId);

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.IsType<InvalidOperationException>(response.InnerException);
            Assert.Equal("The application has been submitted", response.ErrorMessage);

            _fileRecordRepository.Verify(r =>
                r.GetByApplicationIdAsync(It.IsAny<Guid>()), Times.Never);

            _blobStorageService.Verify(b =>
                b.DeleteAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            _applicationRepository.Verify(r =>
                r.DeleteAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
        });
    }

    [Fact]
    public async Task Handle_BlobDeleteThrows_ReturnsError_AndDoesNotDeleteApplication()
    {
        var application = new Data.Entities.Application.Application
        {
            Id = ApplicationId,
            Submitted = false
        };

        var files = new List<FileRecord>
        {
            new() { Id = Guid.NewGuid(), BlobContainer = "container", BlobPath = "path/one.pdf" }
        };

        _applicationRepository
            .Setup(r => r.GetByIdAsync(ApplicationId))
            .ReturnsAsync(application);

        _fileRecordRepository
            .Setup(r => r.GetByApplicationIdAsync(ApplicationId))
            .ReturnsAsync(files);

        _blobStorageService
            .Setup(b => b.DeleteAsync(files[0].BlobContainer, files[0].BlobPath))
            .ThrowsAsync(new Exception(ExceptionMessage));

        var command = new DeleteApplicationCommand(ApplicationId);

        var response = await _handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            Assert.False(response.Success);
            Assert.NotNull(response.InnerException);
            Assert.Equal(ExceptionMessage, response.ErrorMessage);

            _fileRecordRepository.Verify(r =>
                r.DeleteAsync(It.IsAny<Guid>()), Times.Never);

            _applicationRepository.Verify(r =>
                r.DeleteAsync(It.IsAny<Data.Entities.Application.Application>()), Times.Never);
        });
    }
}
