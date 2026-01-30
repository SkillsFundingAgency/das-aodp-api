using Microsoft.OpenApi.Models;
using SFA.DAS.AODP.Api.Extensions;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Queries.Qualifications;
using SFA.DAS.AODP.Application.Swashbuckle;
using SFA.DAS.AODP.Data.Search;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Api;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration.LoadConfiguration(builder.Services, builder.Environment.IsDevelopment());

        // Add services to the container.
        builder.Services
            .AddServiceRegistrations(configuration)
            .AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(
                    typeof(GetNewQualificationsQueryHandler).Assembly,
                    typeof(CreateFormVersionCommandHandler).Assembly))
            .AddLogging()
            .AddDataProtectionKeys("das-aodp-api", configuration, builder.Environment.IsDevelopment())
            .AddHttpContextAccessor()
            .AddHealthChecks();

        builder.Services.AddAuthentication(configuration);

        builder.Services.AddControllers();

        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(c =>
            {
                //Helps with schema's of the same name by adding a number at the end
                var schemaHelper = new SwashbuckleSchemaHelper();
                c.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AODP Inner API", Version = "v1" });
            });


        var connectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

        if (!string.IsNullOrEmpty(connectionString))
        {
            builder.Services.AddApplicationInsightsTelemetry();
        }

        var app = builder.Build();

        var fuzzySearchEnabled = configuration.GetSection("FuzzySearchSettings").GetValue<bool>("Enabled");

        if (fuzzySearchEnabled)
        {
            using (var scope = app.Services.CreateScope())
            {
                var indexBuilder = scope.ServiceProvider.GetRequiredService<IIndexBuilder>();
                indexBuilder.Build();
            }
        }

        app.UseSwagger();
        app.UseSwaggerUI();
        app
            .UseHealthChecks("/ping")
            .UseHttpsRedirection()
            .UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}