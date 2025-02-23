﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.EntityConfiguration;
using System.Reflection.Emit;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Context
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            this._configuration = configuration;
        }

        public virtual DbSet<ApprovedQualificationsImport> ApprovedQualificationsImports { get; set; }

        public virtual DbSet<ProcessedRegisteredQualification> ProcessedRegisteredQualifications { get; set; }

        public virtual DbSet<RegisteredQualificationsImport> RegisteredQualificationsImports { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormVersion> FormVersions { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionOption> QuestionOptions { get; set; }
        public virtual DbSet<QuestionValidation> QuestionValidations { get; set; }
        public virtual DbSet<Route> Routes { get; set; }

        public virtual DbSet<View_AvailableQuestionsForRouting> View_AvailableQuestionsForRoutings { get; set; }
        public virtual DbSet<View_QuestionRoutingDetail> View_QuestionRoutingDetails { get; set; }
        public virtual DbSet<View_SectionPageCount> View_SectionPageCounts { get; set; }

        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationPage> ApplicationPages { get; set; }
        public DbSet<ApplicationQuestionAnswer> ApplicationQuestionAnswers { get; set; }
        public DbSet<View_RemainingPagesBySectionForApplication> View_RemainingPagesBySectionForApplications { get; set; }
        public DbSet<View_SectionSummaryForApplication> View_SectionSummaryForApplications { get; set; }

        public DbSet<QualificationNewReviewRequired> QualificationNewReviewRequired { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QualificationNewReviewRequired>().ToView("v_QualificationNewReviewRequired", "regulated").HasNoKey();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(View_AvailableQuestionsForRoutingEntityConfiguration).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public async Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            await this.BulkInsertAsync(entities.ToList(), options => options.BatchSize = 1000, cancellationToken: cancellationToken);
        }

        public async Task<IDbContextTransaction> StartTransactionAsync()
        {
            return await Database.BeginTransactionAsync();
        }

    }
}

