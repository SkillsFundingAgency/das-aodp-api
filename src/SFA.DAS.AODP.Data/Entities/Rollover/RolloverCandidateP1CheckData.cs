using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Entities.Rollover;

public record RolloverCandidateP1CheckData(
    RolloverCandidateDto Candidate,
    RolloverWorkflowCandidatesP1Checks P1Checks);
