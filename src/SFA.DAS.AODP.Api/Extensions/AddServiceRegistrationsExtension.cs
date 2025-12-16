using Microsoft.Extensions.Options;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Extensions;
using SFA.DAS.AODP.Data.Search;
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

        services.AddScoped<IQualificationsSearchService, QualificationsSearchService>();
        services.AddSingleton<IDirectoryFactory>(new DirectoryFactory());
        services.AddScoped<IIndexBuilder, QualificationsIndexBuilder>();
        services.AddScoped<ISearchManager, QualificationsSearchManager>();

        services.ConfigureDatabase(configuration);

        return services;
    }
}
