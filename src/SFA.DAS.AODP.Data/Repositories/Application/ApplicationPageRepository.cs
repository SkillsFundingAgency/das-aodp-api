using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Entities.FormBuilder;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationPageRepository : IApplicationPageRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationPageRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Data.Entities.Application.ApplicationPage> Create(Data.Entities.Application.ApplicationPage application)
        {
            application.Id = Guid.NewGuid();
            await _context.ApplicationPages.AddAsync(application);
            await _context.SaveChangesAsync();

            return application;
        }


        public async Task<Data.Entities.Application.ApplicationPage> Update(Data.Entities.Application.ApplicationPage application)
        {
            _context.ApplicationPages.Update(application);
            await _context.SaveChangesAsync();

            return application;
        }

        public async Task UpsertAsync(ApplicationPage page)
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

            await _context.SaveChangesAsync();
        }

        public async Task<Data.Entities.Application.ApplicationPage?> GetApplicationPageByPageIdAsync(Guid applicationId, Guid pageId)
        {
            return await _context
                .ApplicationPages
                .Include(x => x.QuestionAnswers)
                .FirstOrDefaultAsync(v => v.PageId == pageId && v.ApplicationId == applicationId);
        }
    }
}
