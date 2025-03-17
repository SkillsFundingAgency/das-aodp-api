using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using ChangedQualification = Entities.Qualification.ChangedQualification;

public interface IQualificationsRepository
{
    Task AddQualificationDiscussionHistory(Entities.Qualification.QualificationDiscussionHistory qualificationDiscussionHistory, string qualificationReference);
    Task<List<ChangedQualification>> GetChangedQualificationsAsync();
    Task UpdateQualificationStatus(string qualificationReference, string status);
}
