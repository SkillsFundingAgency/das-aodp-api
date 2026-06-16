using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Rollover
{
    [ExcludeFromCodeCoverage]
    public record CandidateKey(
        string Qan,
        string FundingStream);

    [ExcludeFromCodeCoverage]
    public record QualificationFundingKey(
        Guid QualificationVersionId,
        Guid FundingOfferId);

    [ExcludeFromCodeCoverage]
    public record FundingExtensionCandidateValidationContext(
        HashSet<CandidateKey> IncomingCandidates,
        HashSet<CandidateKey> CandidatesInDb,
        HashSet<CandidateKey> WorkflowCandidatesInDb
    );

}