using SFA.DAS.AODP.Data.Extensions;
using SFA.DAS.AODP.Models.Settings;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.AODP.Infrastructure.Extensions;

namespace SFA.DAS.AODP.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class AddServiceRegistrationsExtension
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddSingleton(configuration);

        var formBuilderSettings = configuration.GetRequiredSection("FormBuilderSettings").Get<FormBuilderSettings>();
        if (formBuilderSettings != null) services.AddSingleton(formBuilderSettings);

        var blobStorageSettings = configuration.GetRequiredSection("OutputFileBlobStorageSettings").Get<OutputFileBlobStorageSettings>();
        if (blobStorageSettings != null) services.AddSingleton(blobStorageSettings);

        services.ConfigureDatabase(configuration);

        services.AddBlobStorage(configuration);
        return services;
    }
}
