using Microsoft.EntityFrameworkCore;
using SFA.DAS.AODP.Data.Context;
using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Data.Exceptions;
using SFA.DAS.AODP.Models.Application;
using System.Linq;

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
            ApplicationReviewSearchCriteria criteria
        )
        {
            var query = _context
                .ApplicationReviewFeedbacks
                .Include(a => a.ApplicationReview)
                .ThenInclude(a => a.Application)
                .Where(a => a.Type == criteria.ReviewType.ToString());

            if (!string.IsNullOrWhiteSpace(criteria.ApplicationSearch))
            {
                query = query.Where(q =>
                    q.ApplicationReview.Application.ReferenceId.ToString().Contains(criteria.ApplicationSearch.TrimStart('0'))
                    || q.ApplicationReview.Application.Name.Contains(criteria.ApplicationSearch)
                    || q.ApplicationReview.Application.QualificationNumber.Contains(criteria.ApplicationSearch)

                 );
            }

            if (!string.IsNullOrWhiteSpace(criteria.AwardingOrganisationSearch))
            {
                query = query.Where(q =>
                    q.ApplicationReview.Application.AwardingOrganisationName.Contains(criteria.AwardingOrganisationSearch)
                    || q.ApplicationReview.Application.AwardingOrganisationUkprn.Contains(criteria.AwardingOrganisationSearch)
                 );
            }


            if (criteria.ReviewType == UserType.Ofqual)
            {
                query = query.Where(a => a.ApplicationReview.SharedWithOfqual);
            }
            else if (criteria.ReviewType == UserType.SkillsEngland)
            {
                query = query.Where(a => a.ApplicationReview.SharedWithSkillsEngland);

            }

            if (criteria.IncludeApplicationWithNewMessages && criteria.ApplicationStatuses ?.Any() == true)
            {
                query = query.Where(a => a.NewMessage || criteria.ApplicationStatuses.Contains(a.Status));
            }
            else if (criteria.IncludeApplicationWithNewMessages)
            {
                query = query.Where(q => q.NewMessage);
            }
            else if (criteria.ApplicationStatuses?.Any() == true)
            {
                query = query.Where(q => criteria.ApplicationStatuses.Contains(q.Status));
            }

            if (criteria.UnassignedOnly)
            {
                query = query.Where(q =>
                    string.IsNullOrEmpty(q.ApplicationReview.Application.Reviewer1) &&
                    string.IsNullOrEmpty(q.ApplicationReview.Application.Reviewer2));
            }
            else if (!string.IsNullOrWhiteSpace(criteria.ReviewerSearch))
            {
                var term = criteria.ReviewerSearch.Trim();
                query = query.Where(q =>
                    q.ApplicationReview.Application.Reviewer1 == term ||
                    q.ApplicationReview.Application.Reviewer2 == term);
            }

            return (await query.OrderByDescending(o => o.ApplicationReview.Application.UpdatedAt).Skip(criteria.Offset).Take(criteria.Limit).ToListAsync(), await query.CountAsync());

        }

        public async Task<ApplicationReviewFeedback> CreateAsync(ApplicationReviewFeedback applicationReviewFeedback)
        {
            applicationReviewFeedback.Id = Guid.NewGuid();

            await _context.ApplicationReviewFeedbacks.AddAsync(applicationReviewFeedback);
            await _context.SaveChangesAsync();

            return applicationReviewFeedback;
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

        public async Task<ApplicationReviewFeedback> GeyByReviewIdAndUserType(Guid applicationReviewId, UserType userType)
        {
            var res = await _context
                            .ApplicationReviewFeedbacks

                            .Include(a => a.ApplicationReview)
                            .ThenInclude(a => a.ApplicationReviewFundings)
                            .ThenInclude(a => a.FundingOffer)

                            .Include(r => r.ApplicationReview)
                            .ThenInclude(r => r.Application)

                            .FirstOrDefaultAsync(v => v.ApplicationReviewId == applicationReviewId && v.Type == userType.ToString());

            return res is null ? throw new RecordNotFoundException(applicationReviewId) : res;
        }

    }
}
