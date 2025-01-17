using Microsoft.OpenApi.Models;
using SFA.DAS.AODP.Api.Extensions;
using SFA.DAS.AODP.Application.Queries.Test;
using SFA.DAS.AODP.Common.Extensions;
using SFA.DAS.AODP.Infrastructure.Context;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Application.AutoMapper.Profiles;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration.LoadConfiguration(builder.Services, builder.Environment.IsDevelopment());

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add services to the container.
builder.Services
    .AddServiceRegistrations(configuration)
    .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<TestQueryHandler>())
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
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "AODP API", Version = "v1" });
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
    var formDb = new Form { Id = Guid.NewGuid(), Archived = true };
    context.Forms.Add(formDb);

    context.Forms.Add(new Form { Id = Guid.NewGuid(), Archived = false });
    context.FormVersions.Add(new()
    {
        Id = Guid.NewGuid(),
        DateCreated = DateTime.Now,
        FormId = formDb.Id,
        Description = "Something",
        Status = FormStatus.Published,
        Archived = false,
        Name = "Name",
        Version = DateTime.Now,
    });
    context.SaveChangesAsync().Wait();
}

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


