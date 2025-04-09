using SFA.DAS.AODP.Data.Entities.Qualification;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

public interface IQualificationDetailsRepository
{
    Task<List<QualificationDiscussionHistory>> GetDiscussionHistoriesForQualificationRef(string qualificationRef);
    Task<QualificationVersions> GetQualificationDetailsByIdAsync(string qualificationReference);

    Task<QualificationVersions> GetVersionByIdAsync(string qualificationReference, int version);
}
