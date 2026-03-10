using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover
{
    public class GetRolloverCandidatesQueryResponse
    {
        public IEnumerable<RolloverCandidate> RolloverCandidates { get; set; } = new List<RolloverCandidate>();
    }
}