using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Concurrent;
namespace SFA.DAS.AODP.Infrastructure;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ConcurrentDictionary<string, BlobContainerClient> _containers = new();

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task UploadFileAsync(
        string containerName,
        string fileName,
        Stream content,
        string? contentType,
        CancellationToken cancellationToken = default)
    {
        var container = await GetContainerAsync(containerName, cancellationToken);

        var blob = container.GetBlobClient(fileName);
        if (content.CanSeek) content.Position = 0;

        var options = new BlobUploadOptions
        {
            HttpHeaders = string.IsNullOrEmpty(contentType)
                ? null
                : new BlobHttpHeaders { ContentType = contentType }
        };

        var response = await blob.UploadAsync(content, options, cancellationToken);
    }

    private async Task<BlobContainerClient> GetContainerAsync(string containerName, CancellationToken cancellationToken)
    {
        if (_containers.TryGetValue(containerName, out var existing))
        {
            return existing;
        }

        var container = _blobServiceClient.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        _containers[containerName] = container;

        return container;
    }
}
