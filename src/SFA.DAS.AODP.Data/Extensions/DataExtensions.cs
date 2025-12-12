using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Repositories.Application;
using SFA.DAS.AODP.Data.Repositories.Feedback;
using SFA.DAS.AODP.Data.Repositories.FormBuilder;
using SFA.DAS.AODP.Data.Repositories.FundingOffer;
using SFA.DAS.AODP.Data.Repositories.Jobs;
using SFA.DAS.AODP.Data.Repositories.Qualification;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Data.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class DataExtensions
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            {
               var  connectionString = configuration["AodpApi:DatabaseConnectionString"];

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    connectionString = configuration.GetConnectionString("SQLSeverConnectionString");
                }
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new Exception("Database connection string not found");
                }
                options.UseSqlServer(connectionString);

            });
            services.AddScoped<IFormVersionRepository, FormVersionRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<IPageRepository, PageRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuestionValidationRepository, QuestionValidationRepository>();
            services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IQualificationsRepository, QualificationsRepository>();
            services.AddScoped<IQualificationDetailsRepository, QualificationDetailsRepository>();
            services.AddScoped<IQualificationFundingFeedbackRepository, QualificationFundingFeedbackRepository>();
            services.AddScoped<IQualificationFundingsRepository, QualificationFundingsRepository>();
            services.AddScoped<IQualificationDiscussionHistoryRepository, QualificationDiscussionHistoryRepository>();
            services.AddScoped<IChangedQualificationsRepository, ChangedQualificationsRepository>();

            services.AddScoped<IFundingOfferRepository, FundingOfferRepository>();
            services.AddScoped<IApplicationReviewFundingRepository, ApplicationReviewFundingRepository>();
            services.AddScoped<ISurveyRepository, SurveyRepository>();

            services.AddScoped<IFundingOfferRepository, FundingOfferRepository>();
            services.AddScoped<IApplicationReviewFundingRepository, ApplicationReviewFundingRepository>();

            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IApplicationReviewRepository, ApplicationReviewRepository>();
            services.AddScoped<IApplicationReviewFeedbackRepository, ApplicationReviewFeedbackRepository>();
            services.AddScoped<IApplicationPageRepository, ApplicationPageRepository>();
            services.AddScoped<IApplicationQuestionAnswerRepository, ApplicationQuestionAnswerRepository>();
            services.AddScoped<IApplicationMessagesRepository, ApplicationMessagesRepository>();
            services.AddScoped<INewQualificationsRepository, NewQualificationsRepository>();
            services.AddScoped<IJobsRepository, JobsRepository>();
            services.AddScoped<IJobRunsRepository, JobRunsRepository>();

            services.AddScoped<IQualificationOutputFileRepository, QualificationOutputFileRepository>();
            services.AddScoped<IQualificationOutputFileLogRepository, QualificationOutputFileLogRepository>();
            return services;
        }
    }
}
