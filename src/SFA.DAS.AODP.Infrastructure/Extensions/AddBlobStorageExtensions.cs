using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AODP.Models.Settings;

namespace SFA.DAS.AODP.Infrastructure.Extensions
{
    public static class AddBlobStorageExtensions
    {
        public static IServiceCollection AddBlobStorage(this IServiceCollection services, StorageSettings storageSettings)
        {
            services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddBlobServiceClient(new Uri(storageSettings.ServiceUri));
            });

            services.AddTransient<IBlobStorageService, BlobStorageService>();
            return services;

        }
    }
}
