using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationReviewFeedbackRepository
    {
        Task CreateAsync(ApplicationReviewFeedback applicationReviewFeedback);
        Task CreateAsync(List<ApplicationReviewFeedback> applicationReviewFeedbacks);
        Task<ApplicationReviewFeedback> GetApplicationReviewFeedbackDetailsByReviewIdAsync(Guid applicationReviewId, UserType userType);
        Task<(List<ApplicationReviewFeedback>, int)> GetApplicationReviews(UserType reviewType, int offset, int limit, bool includeApplicationWithNewMessages, List<string>? applicationStatuses, string? applicationSearch, string? awardingOrganisationSearch);
        Task<ApplicationReviewFeedback> GetByIdAsync(Guid id);
        Task RemoveAsync(List<ApplicationReviewFeedback> applicationReviewFeedback);
        Task UpdateAsync(ApplicationReviewFeedback applicationReviewFeedback);
        Task UpdateAsync(List<ApplicationReviewFeedback> applicationReviewFeedbacks);
        Task UpsertAsync(List<ApplicationReviewFeedback> applicationReviewFeedback);
    }
}