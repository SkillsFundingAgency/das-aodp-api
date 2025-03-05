using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using ChangedQualification = Entities.Qualification.ChangedQualification;

public interface IChangedQualificationsRepository
{
    Task<ChangedQualificationsResult> GetChangedQualificationsAsync(int? skip = 0, int? take = 0, NewQualificationsFilter? filter = default);
}
