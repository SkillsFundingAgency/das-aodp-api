using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification
{
    public interface IQualificationFundingsRepository
    {
        Task CreateAsync(List<QualificationFundings> qualificationFundings);
        Task<List<QualificationFundings>> GetByIdAsync(Guid applicationReviewId);
        Task RemoveAsync(List<QualificationFundings> qualificationFundings);
        Task UpdateAsync(List<QualificationFundings> qualificationFundings);
    }
}