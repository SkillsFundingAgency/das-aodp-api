﻿using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Infrastructure.Context
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

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class;


    }
}
