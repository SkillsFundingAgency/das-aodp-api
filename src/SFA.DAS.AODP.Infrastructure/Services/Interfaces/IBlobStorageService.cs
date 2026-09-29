namespace SFA.DAS.AODP.Infrastructure.Services.Interfaces
{
    public interface IBlobStorageService
    {
        Task DeleteAsync(string containerName, string blobPath);
    }
}
