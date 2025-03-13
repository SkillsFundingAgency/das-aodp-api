using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationReviewFundingRepository : IApplicationReviewFundingRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationReviewFundingRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationReviewFunding>> GetByReviewIdAsync(Guid applicationReviewId)
        {
            return await _context
                        .ApplicationReviewFundings
                        .Include(a => a.FundingOffer)
                        .Where(v => v.ApplicationReviewId == applicationReviewId)
                        .ToListAsync();

        }

        public async Task UpdateAsync(List<ApplicationReviewFunding> applicationReviewFeedback)
        {
            _context.ApplicationReviewFundings.UpdateRange(applicationReviewFeedback);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(List<ApplicationReviewFunding> applicationReviewFeedback)
        {
            _context.ApplicationReviewFundings.RemoveRange(applicationReviewFeedback);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(List<ApplicationReviewFunding> applicationReviewFeedbacks)
        {
            foreach (var feedback in applicationReviewFeedbacks)
            {
                feedback.Id = Guid.NewGuid();
                _context.ApplicationReviewFundings.Add(feedback);
            }
            await _context.SaveChangesAsync();
        }
    }
}
