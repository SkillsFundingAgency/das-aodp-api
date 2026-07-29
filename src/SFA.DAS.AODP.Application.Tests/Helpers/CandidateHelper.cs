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
            var sourceQualificationId = qualificationVersionId ?? Guid.NewGuid();
            var discussionQualificationId = qualificationId ?? Guid.NewGuid();

            var candidate = fixture.Build<RolloverCandidates>()
                .Do(x =>
                {
                    typeof(RolloverCandidates)
                        .GetProperty(nameof(RolloverCandidates.SourceType))!
                        .SetValue(x, RolloverSourceTypes.Ofqual);

                    typeof(RolloverCandidates)
                        .GetProperty(nameof(RolloverCandidates.SourceQualificationId))!
                        .SetValue(x, sourceQualificationId);
                })
                .Create();

            candidate.FundingOffer.Name = fundingStreamName;
            candidate.SetSourceContext(qan, discussionQualificationId);

            return candidate;
        }
    }
}
