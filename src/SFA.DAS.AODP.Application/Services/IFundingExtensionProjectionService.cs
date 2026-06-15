using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Services
{
    public interface IFundingExtensionProjectionService
    {
        FundingExtensionSummary ProjectSummary(
            List<FundingExtensionCandidateItem> dbCandidates,
            List<FundingExtensionCandidate> uploadedCandidates);
    }

}
