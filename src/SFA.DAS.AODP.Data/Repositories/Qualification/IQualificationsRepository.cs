using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using ChangedQualification = Entities.Qualification.ChangedQualification;

public interface IQualificationsRepository
{
    Task<List<ChangedQualification>> GetChangedQualificationsAsync();

    Task<IEnumerable<ChangedQualificationExport>> GetChangedQualificationsExport();
}
