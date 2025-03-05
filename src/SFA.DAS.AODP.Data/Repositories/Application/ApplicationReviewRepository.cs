using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Exceptions;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationReviewRepository : IApplicationReviewRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationReviewRepository(IApplicationDbContext context)
        {
            _context = context;
        }


        public async Task CreateAsync(ApplicationReview applicationReview)
        {
            applicationReview.Id = Guid.NewGuid();

            _context.ApplicationReviews.Add(applicationReview);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationReview applicationReview)
        {
            _context.ApplicationReviews.Update(applicationReview);
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationReview?> GetByApplicationIdAsync(Guid id)
        {
            return await _context.ApplicationReviews.Include(a => a.ApplicationReviewFeedbacks).FirstOrDefaultAsync(a => a.ApplicationId == id);
        }

        public async Task<ApplicationReview> GetByIdAsync(Guid id)
        {
            return await _context.ApplicationReviews.Include(a => a.ApplicationReviewFeedbacks).FirstOrDefaultAsync(a => a.Id == id) ?? throw new RecordNotFoundException(id);
        }
    }
}
