
namespace SFA.DAS.AODP.Models.Rollover
{
    public class FundingExtensionCandidateItem
    {
        public string Qan { get; set; } = null!;
        public string FundingStreamName { get; set; } = null!;
        public RolloverStatus RolloverStatus { get; set; } = RolloverStatus.None;
    }
}
