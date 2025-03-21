using Microsoft.Extensions.Options;
using SFA.DAS.AODP.Data.Extensions;
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

        services.ConfigureDatabase(configuration);
        return services;
    }
}
