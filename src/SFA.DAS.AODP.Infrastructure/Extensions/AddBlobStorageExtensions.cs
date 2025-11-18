using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.AODP.Infrastructure.Extensions
{
    public static class AddBlobStorageExtensions
    {
        public static IServiceCollection AddBlobStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddBlobServiceClient(configuration.GetValue<string>("OutputFileBlobStorageSettings:ConnectionString"));
            });

            services.AddTransient<IBlobStorageService, BlobStorageService>();
            return services;

        }
    }
}
