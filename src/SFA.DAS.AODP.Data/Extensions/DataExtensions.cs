using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Repositories;
using SFA.DAS.AODP.Infrastructure.Context;

namespace SFA.DAS.AODP.Data.Extensions
{
    public static class DataExtensions
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services)
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
                //options.UseInMemoryDatabase("AODB");
                //options.UseSeeding((context, _) =>
                //{
                //    var formDb = new Form { Id = Guid.NewGuid(), Archived = true };
                //    context.Set<Form>().Add(formDb);

                //    context.Set<Form>().Add(new Form { Id = Guid.NewGuid(), Archived = false });
                //    context.Set<FormVersion>().Add(new()
                //    {
                //        Id = Guid.NewGuid(),
                //        DateCreated = DateTime.Now,
                //        FormId = formDb.Id,
                //        Description = "Something",
                //        Status = FormStatus.Published,
                //        Name = "Name",
                //        Version = DateTime.Now,
                //    });
                //    context.SaveChangesAsync().Wait();
                //});
                //options.UseAsyncSeeding(async (context, _, cancellationToken) =>
                //{
                //    var formDb = new Form { Id = Guid.NewGuid(), Archived = true };
                //    context.Set<Form>().Add(formDb);

                //    context.Set<Form>().Add(new Form { Id = Guid.NewGuid(), Archived = false });
                //    context.Set<FormVersion>().Add(new()
                //    {
                //        Id = Guid.NewGuid(),
                //        DateCreated = DateTime.Now,
                //        FormId = formDb.Id,
                //        Description = "Something",
                //        Status = FormStatus.Published,
                //        Name = "Name",
                //        Version = DateTime.Now,
                //    });
                //    await context.SaveChangesAsync();
                //});
            });
            services.AddScoped<IFormVersionRepository, FormVersionRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<IPageRepository, PageRepository>();

            return services;
        }
    }
}
