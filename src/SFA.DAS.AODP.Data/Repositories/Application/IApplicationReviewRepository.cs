using SFA.DAS.AODP.Data.Entities.Application;

namespace SFA.DAS.AODP.Data.Repositories.Application
{
    public interface IApplicationReviewRepository
    {
        Task CreateAsync(ApplicationReview applicationReview);
        Task<ApplicationReview> GetApplicationForReviewByReviewIdAsync(Guid applicationReviewId);
        Task<ApplicationReview?> GetByApplicationIdAsync(Guid id);
        Task<ApplicationReview> GetByIdAsync(Guid id);
        Task UpdateAsync(ApplicationReview applicationReview);
    }
}