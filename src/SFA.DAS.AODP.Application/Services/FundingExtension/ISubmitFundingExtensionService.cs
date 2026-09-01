using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Data.Entities.Qualification;
using SFA.DAS.AODP.Data.Entities.Rollover;

namespace SFA.DAS.AODP.Application.Services.FundingExtension
{
    public interface ISubmitFundingExtensionService
    {
        Task<bool> Submit(
            List<RolloverCandidates> candidates,
            List<FundingExtensionItem> inputItems,
            List<QualificationFundings> fundings,
            CancellationToken cancellationToken);
    }

}
