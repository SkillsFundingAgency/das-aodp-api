using SFA.DAS.AODP.Data.Entities;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Qualifications;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using ChangedQualification = Entities.Qualification.ChangedQualification;

public interface IChangedQualificationsRepository
{
    Task<ChangedQualificationsResult> GetAllChangedQualificationsAsync(int? skip = 0, int? take = 0, QualificationsFilter? filter = default);
    Task<List<ChangedExport>> GetChangedQualificationsCSVExport();
    Task<QualificationDetails?> GetQualificationDetailsByIdAsync(string qualificationReference);
    Task<List<Entities.Qualification.ActionType>> GetActionTypes();
}
