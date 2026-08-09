using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Data.Repositories.Rollover;

public sealed record RolloverFundingEligibility(
    FundingChangeKey Key,
    string AcademicYear,
    DateOnly? FundingEndDate,
    bool IsEligible);
