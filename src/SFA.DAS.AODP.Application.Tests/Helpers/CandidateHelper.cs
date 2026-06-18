using AutoFixture;
using SFA.DAS.AODP.Data.Entities.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Helpers
{
    public static class CandidateHelper
    {
        public static RolloverCandidates BuildCandidate(
                IFixture fixture,
                string qan,
                string fundingStreamName)
        {
            var candidate = fixture.Create<RolloverCandidates>();
            candidate.QualificationVersion.Qualification.Qan = qan;
            candidate.FundingOffer.Name = fundingStreamName;

            return candidate;
        }
    }
}
