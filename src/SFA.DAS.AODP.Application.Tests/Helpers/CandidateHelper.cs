using AutoFixture;
using SFA.DAS.AODP.Data.Entities.Rollover;

namespace SFA.DAS.AODP.Application.UnitTests.Helpers
{
    public static class CandidateHelper
    {
        public static RolloverCandidates BuildCandidate(
            IFixture fixture,
            string qan,
            string fundingStreamName,
            Guid? qualificationVersionId = null,
            Guid? qualificationId = null)

        {
            var candidate = fixture.Build<RolloverCandidates>()
                .Do(x =>
                {
                    typeof(RolloverCandidates)
                        .GetProperty(nameof(RolloverCandidates.QualificationVersionId))!
                        .SetValue(x, qualificationVersionId);
                })
                .Create();

            candidate.QualificationVersion.Qualification.Qan = qan;
            candidate.QualificationVersion.Id = qualificationVersionId ?? Guid.Empty;
            candidate.QualificationVersion.QualificationId = qualificationId ?? Guid.Empty;
            candidate.FundingOffer.Name = fundingStreamName;

            return candidate;
        }
    }
}
