using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationPageRepository : IApplicationPageRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationPageRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationPage> Create(ApplicationPage application)
        {
            application.Id = Guid.NewGuid();

            await _context.ApplicationPages.AddAsync(application);
            await _context.SaveChangesAsync();

            return application;
        }


        public async Task<ApplicationPage> Update(ApplicationPage application)
        {
            _context.ApplicationPages.Update(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<ApplicationPage?> GetApplicationPageByPageIdAsync(Guid applicationId, Guid pageId)
        {
            return await _context
                .ApplicationPages
                .Include(x => x.QuestionAnswers)
                .FirstOrDefaultAsync(v => v.PageId == pageId && v.ApplicationId == applicationId);
        }

        public async Task<List<ApplicationPage>> GetSkippedApplicationPagesByQuestionIdAsync(Guid applicationId, Guid questionId, List<Guid> pageIdsToIgnore)
        {
            return await _context.ApplicationPages.Where(a => a.ApplicationId == applicationId && a.SkippedByQuestionId == questionId && !pageIdsToIgnore.Contains(a.PageId)).ToListAsync();
        }

        public async Task<List<ApplicationPage>> GetApplicationPagesByPageIdsAsync(Guid applicationId, List<Guid> pageIds)
        {
            return await _context.ApplicationPages.Where(a => a.ApplicationId == applicationId && pageIds.Contains(a.PageId)).ToListAsync();
        }

        public async Task UpsertAsync(List<ApplicationPage> applicationPagesToUpsert)
        {
            foreach (var page in applicationPagesToUpsert)
            {
                UpsertPage(page);

            }
            await _context.SaveChangesAsync();
        }

        public async Task UpsertAsync(ApplicationPage page)
        {
            UpsertPage(page);

            await _context.SaveChangesAsync();
        }

        private void UpsertPage(ApplicationPage page)
        {
            if (page.Id == default)
            {
                page.Id = Guid.NewGuid();
                _context.ApplicationPages.Add(page);

            }
            else
            {
                _context.ApplicationPages.Update(page);
            }
        }

        public async Task<List<ApplicationPage>> GetBySectionIdAsync(Guid sectionId, Guid applicationId)
        {
            return await _context.ApplicationPages.Where(a => a.ApplicationId == applicationId && a.Page.SectionId == sectionId).ToListAsync();
        }
    }
}
