using Microsoft.AspNetCore.Mvc.Authorization;
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

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add(new AuthorizeFilter());
        });

        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(c =>
            {
                var schemaHelper = new SwashbuckleSchemaHelper();
                c.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AODP Inner API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Paste a JWT here (without the 'Bearer ' prefix)."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
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
            using var scope = app.Services.CreateScope();
            var indexBuilder = scope.ServiceProvider.GetRequiredService<IIndexBuilder>();
            indexBuilder.Build();
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHealthChecks("/ping");
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}