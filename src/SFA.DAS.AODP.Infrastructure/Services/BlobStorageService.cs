using Azure.Storage.Blobs;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
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

    public async Task DeleteAsync(string containerName, string blobPath)
    {
        if (string.IsNullOrWhiteSpace(containerName))
            throw new ArgumentException(nameof(containerName));

        if (string.IsNullOrWhiteSpace(blobPath))
            throw new ArgumentException(nameof(blobPath));

        var container =
            _blobServiceClient.GetBlobContainerClient(containerName);

        var blob =
            container.GetBlobClient(blobPath);

        await blob.DeleteIfExistsAsync();
    }
    
}
