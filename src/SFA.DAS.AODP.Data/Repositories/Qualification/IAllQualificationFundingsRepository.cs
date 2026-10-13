using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

public interface IAllQualificationFundingsRepository
{
    Task<List<AllQualificationFunding>> GetAsync(
        AllQualificationFundingFilter filter,
        CancellationToken cancellationToken);
}
