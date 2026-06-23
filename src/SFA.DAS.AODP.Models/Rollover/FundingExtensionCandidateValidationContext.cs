using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.AODP.Models.Rollover
{
    [ExcludeFromCodeCoverage]
    public record CandidateKey(string Qan, string FundingStream)
    {
        public static CandidateKey Create(string? qan, string? fsn)
        {
            var q = Normalise(qan);
            var f = Normalise(fsn);

            if (string.IsNullOrEmpty(q) || string.IsNullOrEmpty(f))
                throw new InvalidOperationException("QAN and FundingStream cannot be empty.");

            return new CandidateKey(q, f);
        }

        private static string Normalise(string? s) =>
            (s ?? "").Trim().ToUpperInvariant();
    }


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