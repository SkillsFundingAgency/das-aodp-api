using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface IQualificationFundingsRepository
    {
        Task CreateAsync(List<QualificationFundings> qualificationFundings);
        Task<List<QualificationFundings>> GetByIdAsync(Guid applicationReviewId);
        Task RemoveAsync(List<QualificationFundings> qualificationFundings);
        Task UpdateAsync(List<QualificationFundings> qualificationFundings);
        Task<List<QualificationFundings>> GetRolloverQualificationFundingsAsync(
            List<QualificationFundingKey> candidates,
            CancellationToken cancellationToken);
    }
}