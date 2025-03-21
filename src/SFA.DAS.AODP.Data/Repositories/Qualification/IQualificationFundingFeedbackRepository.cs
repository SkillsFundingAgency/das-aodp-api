using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

public interface IQualificationFundingFeedbackRepository
{
    Task<QualificationFundingFeedbacks> CreateAsync(QualificationFundingFeedbacks QualificationFundingFeedback);
    Task<QualificationFundingFeedbacks> GetByIdAsync(Guid qualificationVersionId);
    Task UpdateAsync(QualificationFundingFeedbacks QualificationFundingFeedback);
    Task UpdateAsync(List<QualificationFundingFeedbacks> QualificationFundingFeedbacks);
}