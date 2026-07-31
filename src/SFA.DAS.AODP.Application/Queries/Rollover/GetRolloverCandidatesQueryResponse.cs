using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Queries.Rollover
{
    public class GetRolloverCandidatesQueryResponse
    {
        public IEnumerable<RolloverCandidateDto> RolloverCandidates { get; set; } = new List<RolloverCandidateDto>();
    }
}