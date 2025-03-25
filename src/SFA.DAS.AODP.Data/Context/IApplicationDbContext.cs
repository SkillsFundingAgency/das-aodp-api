using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Entities.Jobs;
using SFA.DAS.AODP.Data.Entities.Offer;
using SFA.DAS.AODP.Data.Entities.Qualification;
using System.Collections.Generic;

namespace SFA.DAS.AODP.Data.Context
{
    public interface IApplicationDbContext
    {
        DbSet<ApprovedQualificationsImport> ApprovedQualificationsImports { get; set; }
        DbSet<ProcessedRegisteredQualification> ProcessedRegisteredQualifications { get; set; }
        DbSet<RegisteredQualificationsImport> RegisteredQualificationsImports { get; set; }
        DbSet<ChangedQualification> ChangedQualifications { get; set; }
        DbSet<QualificationVersions> QualificationVersions { get; set; }
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
        DbSet<Message> Messages { get; set; }
        DbSet<View_SectionPageCount> View_SectionPageCounts { get; set; }
        DbSet<View_RemainingPagesBySectionForApplication> View_RemainingPagesBySectionForApplications { get; set; }
        DbSet<View_SectionSummaryForApplication> View_SectionSummaryForApplications { get; set; }
        DbSet<View_PagesSectionsAssociatedWithRouting> View_PagesSectionsAssociatedWithRoutings { get; set; }
        DbSet<Job> Jobs { get; set; }
        DbSet<JobConfiguration> JobConfigurations { get; set; }
        DbSet<JobRun> JobRuns { get; set; }
        DbSet<ApplicationReview> ApplicationReviews { get; set; }
        DbSet<ApplicationReviewFeedback> ApplicationReviewFeedbacks { get; set; }
        DbSet<FundingOffer> FundingOffers { get; set; }
        DbSet<ApplicationReviewFunding> ApplicationReviewFundings { get; set; }
        DbSet<ActionType> ActionType { get; set; }
        DbSet<LifecycleStage> LifecycleStages { get; set; }
        DbSet<AwardingOrganisation> AwardingOrganisation { get; set; }
        DbSet<ProcessStatus> ProcessStatus { get; set; }
        DbSet<Qualification> Qualification { get; set; }
        DbSet<FundedQualification> FundedQualifications { get; set; }
        DbSet<QualificationDiscussionHistory> QualificationDiscussionHistory { get; set; }
        DbSet<QualificationOffer> QualificationOffers { get; set; }       
        DbSet<VersionFieldChange> VersionFieldChanges { get; set; }
        DbSet<QualificationNewReviewRequired> QualificationNewReviewRequired { get; set; }
        DbSet<NewQualificationExport> NewQualificationExport { get; set; }
        DbSet<ChangedQualificationExport> ChangedQualificationExport { get; set; }

        DbSet<QualificationFundingFeedbacks> QualificationFundingFeedbacks { get; set; }
        DbSet<QualificationFundings> QualificationFundings { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class;
        Task<IDbContextTransaction> StartTransactionAsync();
    }
}
