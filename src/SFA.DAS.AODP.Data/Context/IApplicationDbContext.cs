using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using System.Collections.Generic;

namespace SFA.DAS.AODP.Data.Context
{
    public interface IApplicationDbContext
    {
        DbSet<ApprovedQualificationsImport> ApprovedQualificationsImports { get; set; }
        DbSet<ProcessedRegisteredQualification> ProcessedRegisteredQualifications { get; set; }
        DbSet<RegisteredQualificationsImport> RegisteredQualificationsImports { get; set; }
        DbSet<Form> Forms { get; set; }
        DbSet<FormVersion> FormVersions { get; set; }
        DbSet<Section> Sections { get; set; }
        DbSet<Page> Pages { get; set; }
        DbSet<Question> Questions { get; set; }
        DbSet<QuestionOption> QuestionOptions { get; set; }
        DbSet<QuestionValidation> QuestionValidations { get; set; }
        DbSet<View_AvailableQuestionsForRouting> View_AvailableQuestionsForRoutings { get; set; }
        DbSet<Route> Routes { get; set; }
        DbSet<View_QuestionRoutingDetail> View_QuestionRoutingDetails { get; set; }
        DbSet<Application> Applications { get; }
        DbSet<ApplicationPage> ApplicationPages { get; }
        DbSet<ApplicationQuestionAnswer> ApplicationQuestionAnswers { get; }
        DbSet<View_SectionPageCount> View_SectionPageCounts { get; set; }
        DbSet<View_RemainingPagesBySectionForApplication> View_RemainingPagesBySectionForApplications { get; set; }
        DbSet<View_SectionSummaryForApplication> View_SectionSummaryForApplications { get; set; }
        DbSet<View_PagesSectionsAssociatedWithRouting> View_PagesSectionsAssociatedWithRoutings { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class;
        Task<IDbContextTransaction> StartTransactionAsync();
    }
}
