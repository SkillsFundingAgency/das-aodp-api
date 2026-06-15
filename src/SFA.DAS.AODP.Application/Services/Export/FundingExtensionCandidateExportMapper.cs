using SFA.DAS.AODP.Application.Commands.Rollover;
using SFA.DAS.AODP.Application.Constants;
using SFA.DAS.AODP.Infrastructure.Extensions;
using SFA.DAS.AODP.Models.Rollover;

namespace SFA.DAS.AODP.Application.Services.Export;

public static class FundingExtensionCandidateExportMapper
{
    public static FundingExtensionCandidateDto Map(FundingExtensionCandidate c)
    {
        return new FundingExtensionCandidateDto
        {
            RowNumber = c.RowNumber,
            QAN = c.Qan.OrEmpty(),
            QualificationTitle = c.QualificationTitle.OrEmpty(),
            AwardingOrganisation = c.AwardingOrganisation.OrEmpty(),
            QualificationLevel = c.QualificationLevel.OrEmpty(),
            QualificationType = c.QualificationType.OrEmpty(),
            SSA = c.Ssa.OrEmpty(),
            OperationalEndDate = c.OperationalEndDate,
            OfferedInEngland = c.OfferedInEngland.OrFalse(),
            FundedInEngland = c.FundedInEngland.OrFalse(),
            GLH = c.Glh.ToIntOrDefault(),
            TQT = c.Tqt.ToIntOrDefault(),
            Pre16 = c.PreSixteen.OrFalse(),
            Age16To18 = c.SixteenToEighteen.OrFalse(),
            Age18Plus = c.EighteenPlus.OrFalse(),
            Age19Plus = c.NineteenPlus.OrFalse(),
            FundingStreamName = c.FundingStreamName.OrEmpty(),
            FundingApprovalStartDate = c.FundingApprovalStartDate.ToDateOnlyOrNull(),
            ProposedOutcome = c.ProposedOutcome.OrEmpty(),
            RolloverStatus = RolloverStatusInfo.FromCsv(c.RollOverStatus),
            ExclusionReason = c.ExclusionReason,
            CurrentFundingApprovalEndDate = c.CurrentFundingApprovalEndDate ?? default,
            ProposedFundingApprovalEndDate = c.ProposedFundingApprovalEndDate,
            Comments = c.Comments
        };
    }
}
