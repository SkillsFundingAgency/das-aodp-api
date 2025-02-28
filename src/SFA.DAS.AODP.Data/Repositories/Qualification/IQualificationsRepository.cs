using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using ChangedQualification = Entities.Qualification.ChangedQualification;

public interface IQualificationsRepository
{
    Task<List<ChangedQualification>> GetChangedQualificationsAsync();
}
