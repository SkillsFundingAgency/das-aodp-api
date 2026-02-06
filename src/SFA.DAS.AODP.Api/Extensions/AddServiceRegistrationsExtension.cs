using RestEase;
using SFA.DAS.AODP.Application.Services;
using SFA.DAS.AODP.Data.Extensions;
using SFA.DAS.AODP.Infrastructure.Clients.Ofqual;
using SFA.DAS.AODP.Data.Search;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Infrastructure.Services;
using SFA.DAS.AODP.Infrastructure.Services.Interfaces;
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

        var qualificationsApiSettings = configuration.GetRequiredSection("QualificationsApiSettings").Get<QualificationsApiSettings>();
        if (qualificationsApiSettings != null) services.AddSingleton(qualificationsApiSettings);

        services.ConfigureDatabase(configuration);

        services.AddBlobStorage(configuration);

        services.AddScoped<INotificationDefinitionFactory, NotificationDefinitionFactory>();

        services.AddScoped<IOfqualRegisterApi>(provider =>
        {
            var cfg = provider.GetRequiredService<QualificationsApiSettings>();

            var api = RestClient.For<IOfqualRegisterApi>(cfg.BaseUrl);
            api.SubscriptionKey = cfg.ApiKey;

            return api;
        });
        services.AddScoped<IQanValidationService, QanValidationService>();
        services.AddScoped<IQualificationsApi, QualificationsApi>();

        return services;
    }
}
