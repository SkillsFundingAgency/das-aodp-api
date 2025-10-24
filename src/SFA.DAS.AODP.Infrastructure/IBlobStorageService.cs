namespace SFA.DAS.AODP.Infrastructure;
public interface IBlobStorageService
{
    Task UploadFileAsync(
        string containerName,
        string fileName,
        Stream content,
        string? contentType,
        CancellationToken cancellationToken = default);
}

