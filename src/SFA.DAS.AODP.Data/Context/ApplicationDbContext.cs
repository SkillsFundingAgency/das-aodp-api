using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public virtual DbSet<ApprovedQualificationsImport> ApprovedQualificationsImports { get; set; }
        public virtual DbSet<ProcessedRegisteredQualification> ProcessedRegisteredQualifications { get; set; }
        public virtual DbSet<RegisteredQualificationsImport> RegisteredQualificationsImports { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormVersion> FormVersions { get; set; }


        protected override void OnConfiguring
       (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "AODP");


            //optionsBuilder.UseSeeding((context, _) =>
            // {
            //     context.Database.EnsureCreated();
            //     var formDb = new Form { Id = Guid.NewGuid(), IsActive = true };
            //     context.Set<Form>().Add(formDb);

            //     context.Set<Form>().Add(new Form { Id = Guid.NewGuid(), IsActive = false });
            //     context.Set<FormVersion>().Add(new()
            //     {
            //         Id = Guid.NewGuid(),
            //         DateCreated = DateTime.Now,
            //         FormId = formDb.Id,
            //         Description = "Something",
            //         Name = "Name",
            //         Version = "123"
            //     });
            //     context.SaveChanges();

            // });
        }
        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public async Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            await this.BulkInsertAsync(entities.ToList(), options => options.BatchSize = 1000, cancellationToken: cancellationToken);
        }

    }
}

