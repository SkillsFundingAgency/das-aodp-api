using Azure.Storage.Blobs;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using System.Collections.Concurrent;
namespace SFA.DAS.AODP.Infrastructure;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task DeleteAsync(string containerName, string blobPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(containerName);

        ArgumentException.ThrowIfNullOrEmpty(blobPath);

        var container =
            _blobServiceClient.GetBlobContainerClient(containerName);

        var blob =
            container.GetBlobClient(blobPath);

        await blob.DeleteIfExistsAsync();
    }
    
}
