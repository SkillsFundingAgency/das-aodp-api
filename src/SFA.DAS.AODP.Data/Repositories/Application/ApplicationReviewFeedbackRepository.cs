using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public class ApplicationReviewFeedbackRepository : IApplicationReviewFeedbackRepository
    {
        private readonly IApplicationDbContext _context;

        public ApplicationReviewFeedbackRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<ApplicationReviewFeedback>, int)> GetApplicationReviews
        (
            UserType reviewType,
            int offset,
            int limit,
            bool includeApplicationWithNewMessages,
            List<string>? applicationStatuses = null,
            string? applicationSearch = null,
            string? awardingOrganisationSearch = null
        )
        {
            var query = _context
                .ApplicationReviewFeedbacks
                .Include(a => a.ApplicationReview)
                .ThenInclude(a => a.Application)
                .Where(a => a.Type == reviewType.ToString());

            if (!string.IsNullOrWhiteSpace(applicationSearch))
            {
                query = query.Where(q =>
                    q.ApplicationReview.Application.ReferenceId.ToString().Contains(applicationSearch.TrimStart('0'))
                    || q.ApplicationReview.Application.Name.Contains(applicationSearch)
                    || q.ApplicationReview.Application.QualificationNumber.Contains(applicationSearch)

                 );
            }

            if (!string.IsNullOrWhiteSpace(awardingOrganisationSearch))
            {
                query = query.Where(q =>
                    q.ApplicationReview.Application.AwardingOrganisationName.Contains(awardingOrganisationSearch)
                    || q.ApplicationReview.Application.AwardingOrganisationUkprn.Contains(awardingOrganisationSearch)
                 );
            }


            if (reviewType == UserType.Ofqual)
            {
                query = query.Where(a => a.ApplicationReview.SharedWithOfqual);
            }
            else if (reviewType == UserType.SkillsEngland)
            {
                query = query.Where(a => a.ApplicationReview.SharedWithSkillsEngland);

            }

            if (includeApplicationWithNewMessages && applicationStatuses?.Any() == true)
            {
                query = query.Where(a => a.NewMessage || applicationStatuses.Contains(a.Status));
            }
            else if (includeApplicationWithNewMessages)
            {
                query = query.Where(q => q.NewMessage);
            }
            else if (applicationStatuses?.Any() == true)
            {
                query = query.Where(q => applicationStatuses.Contains(q.Status));
            }


            return (await query.OrderByDescending(o => o.ApplicationReview.Application.UpdatedAt).Skip(offset).Take(limit).ToListAsync(), await query.CountAsync());

        }

        public async Task CreateAsync(ApplicationReviewFeedback applicationReviewFeedback)
        {
            applicationReviewFeedback.Id = Guid.NewGuid();

            _context.ApplicationReviewFeedbacks.Add(applicationReviewFeedback);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationReviewFeedback applicationReviewFeedback)
        {
            _context.ApplicationReviewFeedbacks.Update(applicationReviewFeedback);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(List<ApplicationReviewFeedback> applicationReviewFeedback)
        {
            _context.ApplicationReviewFeedbacks.UpdateRange(applicationReviewFeedback);
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationReviewFeedback> GetByIdAsync(Guid id)
        {
            return await _context.ApplicationReviewFeedbacks.FirstOrDefaultAsync(a => a.Id == id) ?? throw new RecordNotFoundException(id);
        }

    }
}
