using Microsoft.OpenApi.Models;
using SFA.DAS.AODP.Api.Extensions;
using SFA.DAS.AODP.Application.Queries.Test;
using SFA.DAS.AODP.Application.Swashbuckle;
using SFA.DAS.AODP.Common.Extensions;
using SFA.DAS.AODP.Infrastructure.Context;
using SFA.DAS.AODP.Application.Queries.Qualifications;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration.LoadConfiguration(builder.Services, builder.Environment.IsDevelopment());

// Add services to the container.
builder.Services
    .AddServiceRegistrations(configuration)
    .AddMediatR(cfg =>
        cfg.RegisterServicesFromAssemblies(
            typeof(TestQueryHandler).Assembly,
            typeof(GetNewQualificationsQueryHandler).Assembly))
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

var app = builder.Build();

#if DEBUG
using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
    await context!.Database.EnsureCreatedAsync();
#endif

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