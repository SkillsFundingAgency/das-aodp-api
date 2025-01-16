using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Infrastructure.Context;

namespace SFA.DAS.AODP.Data.Extensions
{
    public static class DataExtensions
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services)
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>();
            services.AddTransient<IFormRepository, FormRepository>();

            return services;
        }
    }
}
