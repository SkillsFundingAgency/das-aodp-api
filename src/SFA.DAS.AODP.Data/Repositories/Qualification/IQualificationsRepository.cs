using SFA.DAS.AODP.Data.Entities;

namespace SFA.DAS.AODP.Data.Repositories.Qualification;

using Qualification = Entities.Qualification.Qualification;

public interface IQualificationsRepository
{
    Task<List<Qualification>> GetChangedQualificationsAsync();
}
