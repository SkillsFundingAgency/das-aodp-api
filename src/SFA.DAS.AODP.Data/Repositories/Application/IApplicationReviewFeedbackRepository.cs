using SFA.DAS.AODP.Data.Entities.Application;
using SFA.DAS.AODP.Models.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationReviewFeedbackRepository
    {
        Task<ApplicationReviewFeedback> GetByIdAsync(Guid id);
        Task<ApplicationReviewFeedback> CreateAsync(ApplicationReviewFeedback applicationReviewFeedback);
        Task<ApplicationReviewFeedback> GeyByReviewIdAndUserType(Guid applicationReviewId, UserType userType);
        Task<(List<ApplicationReviewFeedback>, int)> GetApplicationReviews(ApplicationReviewSearchCriteria criteria);
        Task UpdateAsync(ApplicationReviewFeedback applicationReviewFeedback);
        Task UpdateAsync(List<ApplicationReviewFeedback> applicationReviewFeedbacks);
    }
}