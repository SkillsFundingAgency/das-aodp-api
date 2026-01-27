using SFA.DAS.AODP.Application.Services;
using SFA.DAS.AODP.Data.Extensions;
using SFA.DAS.AODP.Data.Search;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Models.Settings;
using System.Diagnostics.CodeAnalysis;

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

        services.Configure<FuzzySearchSettings>(configuration.GetSection("FuzzySearchSettings"));
        services.AddScoped<IQualificationsSearchService, QualificationsSearchService>();
        services.AddSingleton<IDirectoryFactory>(new DirectoryFactory());
        services.AddScoped<IIndexBuilder, QualificationsIndexBuilder>();
        services.AddScoped<ISearchManager, QualificationsSearchManager>();

        services.ConfigureDatabase(configuration);

        services.AddBlobStorage(configuration);

        services.AddScoped<INotificationDefinitionFactory, NotificationDefinitionFactory>();

        return services;
    }
}
