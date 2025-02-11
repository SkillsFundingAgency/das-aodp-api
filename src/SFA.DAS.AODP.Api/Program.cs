using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using SFA.DAS.AODP.Api.Extensions;
using SFA.DAS.AODP.Application.Commands.FormBuilder.Forms;
using SFA.DAS.AODP.Application.Swashbuckle;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration.LoadConfiguration(builder.Services, builder.Environment.IsDevelopment());

// Add services to the container.
builder.Services
    .AddServiceRegistrations(configuration)
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateFormVersionCommandHandler>())
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
    builder.Services
        .AddOpenTelemetry()
        .UseAzureMonitor(options => options.ConnectionString = connectionString);
}

builder.Logging.AddApplicationInsights();
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("SFA.DAS", LogLevel.Information);
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app
    .UseHealthChecks("/ping")
    .UseHttpsRedirection()
    .UseAuthorization();

app.MapControllers();

app.Run();