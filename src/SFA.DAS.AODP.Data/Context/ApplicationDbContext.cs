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
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Page> Pages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Page>(entity =>
            {
                entity.OwnsMany(
                    e => e.Questions, ownedNavigationBuilder =>
                    {
                        ownedNavigationBuilder.ToJson();
                        ownedNavigationBuilder.OwnsMany(q => q.RoutingPoints);
                        ownedNavigationBuilder.OwnsMany(q => q.MultiChoice);
                        ownedNavigationBuilder.OwnsOne(q => q.BooleanValidaor);
                        ownedNavigationBuilder.OwnsOne(q => q.DecimalValidator);
                        ownedNavigationBuilder.OwnsOne(q => q.IntegerValidator);
                        ownedNavigationBuilder.OwnsOne(q => q.TextValidator);
                        ownedNavigationBuilder.OwnsOne(q => q.MultiChoiceValidator);
                        ownedNavigationBuilder.OwnsOne(
                            q => q.DateValidator, builder =>
                            {
                                builder.OwnsOne(v => v.GreaterThanTimeInFuture);
                                builder.OwnsOne(v => v.LessThanTimeInFuture);
                                builder.OwnsOne(v => v.GreaterThanTimeInPast);
                                builder.OwnsOne(v => v.LessThanTimeInPast);
                            });
                    }
                );
            });
        }

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

