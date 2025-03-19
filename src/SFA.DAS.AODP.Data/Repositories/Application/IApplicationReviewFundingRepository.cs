using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationReviewFundingRepository
    {
        Task CreateAsync(List<ApplicationReviewFunding> applicationReviewFeedbacks);
        Task<List<ApplicationReviewFunding>> GetByReviewIdAsync(Guid applicationReviewId);
        Task RemoveAsync(List<ApplicationReviewFunding> applicationReviewFeedback);
        Task UpdateAsync(List<ApplicationReviewFunding> applicationReviewFeedback);
    }
}