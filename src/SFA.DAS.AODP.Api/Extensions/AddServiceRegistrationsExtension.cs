using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class AddServiceRegistrationsExtension
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddSingleton<IConfiguration>(configuration);
        return services;
    }
}
