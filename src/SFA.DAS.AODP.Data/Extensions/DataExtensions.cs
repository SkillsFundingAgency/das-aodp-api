using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;

namespace SFA.DAS.AODP.Data.Extensions
{
    public static class DataExtensions
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("SQLSeverConnectionString");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    connectionString = configuration["AodpApi:DatabaseConnectionString"];
                }

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new Exception("Database connection string not found");
                }
                options.UseSqlServer();
            });
            services.AddScoped<IFormVersionRepository, FormVersionRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<IPageRepository, PageRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuestionValidationRepository, QuestionValidationRepository>();
            services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IFormRepository, FormRepository>();

            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IApplicationPageRepository, ApplicationPageRepository>();
            services.AddScoped<IApplicationQuestionAnswerRepository, ApplicationQuestionAnswerRepository>();

            return services;
        }
    }
}
