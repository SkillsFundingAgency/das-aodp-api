namespace SFA.DAS.AODP.Models.Rollover
{
    public record CandidateKey(
        string Qan,
        string FundingStream);

    public record FundingExtensionCandidateValidationContext(
        HashSet<CandidateKey> IncomingCandidates,
        HashSet<CandidateKey> CandidatesInDb,
        HashSet<CandidateKey> WorkflowCandidatesInDb
    );

}