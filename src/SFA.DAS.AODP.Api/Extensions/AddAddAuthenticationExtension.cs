using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class AddAddAuthenticationExtension
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfigurationRoot configuration)
    {
        if (!ConfigurationIsLocalOrDev(configuration))
        {
            var azureAdConfiguration = configuration
                .GetSection("AzureAd")
                .Get<AzureActiveDirectoryConfiguration>();

            var policies = new Dictionary<string, string>
                {
                    {PolicyNames.Default, "Default"}
                };

            services.AddAuthentication(azureAdConfiguration, policies);
        }
        return services;
    }

    private static bool ConfigurationIsLocalOrDev(IConfigurationRoot configuration)
    {
        return configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
               configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
    }
}
