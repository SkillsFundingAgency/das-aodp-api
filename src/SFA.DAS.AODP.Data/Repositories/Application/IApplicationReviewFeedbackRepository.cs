using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationReviewFeedbackRepository
    {
        Task CreateAsync(ApplicationReviewFeedback applicationReviewFeedback);
        Task<(List<ApplicationReviewFeedback>, int)> GetApplicationReviews(UserType reviewType, int offset, int limit, bool includeApplicationWithNewMessages, List<string>? applicationStatuses, string? applicationSearch, string? awardingOrganisationSearch);
        Task<ApplicationReviewFeedback> GetByIdAsync(Guid id);
        Task UpdateAsync(ApplicationReviewFeedback applicationReviewFeedback);
        Task UpdateAsync(List<ApplicationReviewFeedback> applicationReviewFeedbacks);
    }
}