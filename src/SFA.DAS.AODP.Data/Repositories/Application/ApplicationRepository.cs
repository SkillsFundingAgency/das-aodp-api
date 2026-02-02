using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.FormBuilder;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Data.Entities.Application.Application>> GetByOrganisationId(Guid organisationId)
        {
            return await _context.Applications
                .Where(v => v.OrganisationId == organisationId)
                .ToListAsync();

        }

        public async Task<Data.Entities.Application.Application> Create(Data.Entities.Application.Application application)
        {
            application.Id = Guid.NewGuid();
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task DeleteAsync(Entities.Application.Application application)
        {
            await _context.ApplicationQuestionAnswers.Where(t => t.ApplicationPage.ApplicationId == application.Id).ExecuteDeleteAsync();
            await _context.ApplicationPages.Where(t => t.ApplicationId == application.Id).ExecuteDeleteAsync();

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
        }

        public async Task<Entities.Application.Application> GetByIdAsync(Guid applicationId)
        {
            var res = await _context.Applications.FirstOrDefaultAsync(v => v.Id == applicationId);
            return res is null ? throw new RecordNotFoundException(applicationId) : res;
        }

        public async Task<List<View_SectionSummaryForApplication>> GetSectionSummaryByApplicationIdAsync(Guid applicationId)
        {
            return await _context.View_SectionSummaryForApplications.Where(a => a.ApplicationId == applicationId).ToListAsync();
        }

        public async Task UpdateAsync(Entities.Application.Application application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task<Guid> GetFormVersionIdForApplicationAsync(Guid applicationId)
        {
            return await _context.Applications
                .Where(a => a.Id == applicationId)
                .Select(a => a.FormVersionId)
                .FirstOrDefaultAsync();
        }

        public async Task<Entities.Application.Application> GetWithReviewFeedbacksByIdAsync(Guid applicationId)
        {
            var res = await _context.Applications
                .Include(a => a.ApplicationReview)
                    .ThenInclude(r => r.ApplicationReviewFeedbacks)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            return res is null ? throw new RecordNotFoundException(applicationId) : res;
        }

        public async Task<List<Data.Entities.Application.Application>> GetByQan(string qan)
        {
            return await _context.Applications
                .Where(v => v.QualificationNumber == qan)
                .ToListAsync();

        }
    }
}
