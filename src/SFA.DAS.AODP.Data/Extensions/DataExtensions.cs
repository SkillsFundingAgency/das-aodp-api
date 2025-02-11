using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using System.Data.Common;

namespace SFA.DAS.AODP.Data.Extensions
{
    public static class DatabaseExtensions
    {
        private const string AzureResource = "https://database.windows.net/";

        // Take advantage of ChainedTokenCredential's built-in caching
        private static readonly ChainedTokenCredential AzureServiceTokenProvider = new(
            new ManagedIdentityCredential(),
            new AzureCliCredential(),
            new VisualStudioCodeCredential(),
            new VisualStudioCredential());

        public static DbConnection GetSqlConnection(string? connectionString)
        {
            ArgumentNullException.ThrowIfNull(connectionString);

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var useManagedIdentity = !connectionStringBuilder.IntegratedSecurity && string.IsNullOrEmpty(connectionStringBuilder.UserID);

            if (!useManagedIdentity)
            {
                return new SqlConnection(connectionString);
            }

            return new SqlConnection
            {
                ConnectionString = connectionString,
                AccessToken = AzureServiceTokenProvider.GetToken(new TokenRequestContext(scopes: [AzureResource])).Token
            };
        }
    }

    public static class DataExtensions
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                //var connectionString = configuration.GetConnectionString("SQLSeverConnectionString");
                //if (string.IsNullOrWhiteSpace(connectionString))
                //{
                //}
                var connectionString = configuration["AodpApi:DatabaseConnectionString"];

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new Exception("Database connection string not found");
                }


                var connection = DatabaseExtensions.GetSqlConnection(connectionString);
                options.UseSqlServer(connection);

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
