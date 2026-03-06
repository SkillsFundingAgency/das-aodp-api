using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public interface IRolloverRepository
{
    Task<RolloverWorkflowCandidatesResult> GetAllRolloverWorkflowCandidatesAsync(int? skip = 0, int? take = 0);
}
