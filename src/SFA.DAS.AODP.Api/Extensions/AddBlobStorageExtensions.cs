using Microsoft.Extensions.Azure;
using SFA.DAS.AODP.Infrastructure;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class AddBlobStorageExtensions
    {
        public static IServiceCollection AddBlobStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddBlobServiceClient(configuration.GetValue<string>("BlobStorageSettings:ConnectionString"));
            });

            services.AddTransient<IBlobStorageService, BlobStorageService>();
            return services;

        }
    }
}
