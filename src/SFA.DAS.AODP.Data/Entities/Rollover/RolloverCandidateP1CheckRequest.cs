namespace SFA.DAS.AODP.Data.Entities.Rollover;

public record RolloverCandidateP1CheckRequest(
    Guid RolloverCandidateId,
    DateTime? FundingEndDateEligibilityThreshold,
    DateTime? OperationalEndDateEligibilityThreshold,
    DateTime? MaximumApprovalFundingEndDate);
