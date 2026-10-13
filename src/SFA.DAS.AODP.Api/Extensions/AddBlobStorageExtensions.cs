using Microsoft.Extensions.Azure;
using SFA.DAS.AODP.Infrastructure;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using SFA.DAS.AODP.Models.Settings;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Api.Extensions
{
    [ExcludeFromCodeCoverage]
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
